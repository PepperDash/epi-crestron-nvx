using NvxEpi;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NvxEpi
{
    internal class NvxRouter : EssentialsDevice, IMatrixRouting, IRoutingWithFeedback
    {
        public const string DeviceKey = "nvxRouter";
        private static readonly NvxRouter instance = new();
        static NvxRouter() => DeviceManager.AddDevice(instance);

        private NvxRouter() : base(DeviceKey)
        {
        }

        private static readonly Dictionary<string, NvxMatrixRoutingInput> inputs = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, NvxMatrixRoutingOutput> outputs = new(StringComparer.OrdinalIgnoreCase);

        public static void RegisterTransmitter(INvxDevice transmitter)
        {
            var matrixMixerStreamInput = new RoutingInputPort(transmitter.Key, eRoutingSignalType.Video, eRoutingPortConnectionType.Streaming, transmitter, instance);
            var matrixMixerAudioInput = new RoutingInputPort(transmitter.Key, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, transmitter, instance);
            inputs.Add(transmitter.Key, new NvxMatrixRoutingInput(transmitter, matrixMixerStreamInput, matrixMixerAudioInput));

            var streamOutput = transmitter.OutputPorts[NvxDevice.StreamRoutingPortKey];
            if (streamOutput != null)
            {
                instance.InputPorts.Add(matrixMixerStreamInput);
                TieLineCollection.Default.Add(new TieLine(streamOutput, matrixMixerStreamInput));
            }

            var audioOutput = transmitter.OutputPorts[NvxDevice.DmNaxRoutingPortKey];
            if (audioOutput != null)
            {
                instance.InputPorts.Add(matrixMixerAudioInput);
                TieLineCollection.Default.Add(new TieLine(audioOutput, matrixMixerAudioInput));
            }

            instance.LogDebug("Added stream transmitter {transmitter}", transmitter.Key);
        }

        public static void RegisterReceiver(INvxDevice receiver)
        {
            var streamOutput = new RoutingOutputPort(receiver.Key, eRoutingSignalType.Video, eRoutingPortConnectionType.Streaming, receiver, instance);
            var audioOutput = new RoutingOutputPort(receiver.Key, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, receiver, instance);
            outputs.Add(receiver.Key, new NvxMatrixRoutingOutput(receiver, streamOutput, audioOutput));

            var streamInput = receiver.InputPorts[NvxDevice.StreamRoutingPortKey];
            if (streamInput != null)
            {
                instance.OutputPorts.Add(streamOutput);
                TieLineCollection.Default.Add(new TieLine(streamOutput, streamInput));
            }

            var audioInput = receiver.InputPorts[NvxDevice.DmNaxRoutingPortKey];
            if (audioInput != null)
            {
                instance.OutputPorts.Add(audioOutput);
                TieLineCollection.Default.Add(new TieLine(audioOutput, audioInput));
            }

            instance.LogDebug("Added stream receiver {receiver}", receiver.Key);
        }

        private readonly object @lock = new();
        private readonly List<RouteSwitchDescriptor> currentRoutes = new();

        public override bool CustomActivate()
        {
            foreach (var output in outputs.Values)
            {
                var streamPort = output.MatrixMixerStreamOutputPort;
                var audioPort = output.MatrixMixerDmNaxOutputPort;

                var streamRoutes = inputs.Values.Select(input => new { Route = new RouteSwitchDescriptor(streamPort, input.MatrixMixerStreamInputPort), Input = input }).ToList();
                var audioRoutes = inputs.Values.Select(input => new { Route = new RouteSwitchDescriptor(audioPort, input.MatrixMixerDmNaxInputPort), Input = input }).ToList();

                output.Receiver.StreamUrlFeedback.OutputChange += (s, e) =>
                {
                    lock (@lock)
                    {
                        if (streamRoutes.FirstOrDefault(r => r.Input.Transmitter.StreamUrlFeedback.StringValue == e.StringValue) is { Route: RouteSwitchDescriptor route })
                        {
                            var _ = currentRoutes.Remove(route);
                            currentRoutes.Add(route);
                            RouteChanged?.Invoke(this, route);
                        }
                        else
                        {
                            this.LogInformation("Was not able to find stream route for {streamUrl}", e.StringValue);
                        }
                    }
                };

                output.Receiver.DmNaxRxAddressFeedback.OutputChange += (s, e) =>
                {
                    lock (@lock)
                    {
                        if (audioRoutes.FirstOrDefault(r => r.Input.Transmitter.DmNaxTxAddressFeedback.StringValue == e.StringValue) is { Route: RouteSwitchDescriptor route })
                        {
                            var _ = currentRoutes.Remove(route);
                            currentRoutes.Add(route);
                            RouteChanged?.Invoke(this, route);
                        }
                        else
                        {
                            this.LogInformation("Was not able to find audio route for {dmNaxTxAddress}", e.StringValue);
                        }
                    }
                };
            }

            return true;
        }

        public Dictionary<string, IRoutingInputSlot> InputSlots =>
            inputs.ToDictionary(t => t.Key, t => (IRoutingInputSlot) t.Value.RoutingInputSlot, StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, IRoutingOutputSlot> OutputSlots =>
            outputs.ToDictionary(r => r.Key, r => (IRoutingOutputSlot) r.Value.RoutingOutputSlot, StringComparer.OrdinalIgnoreCase);

        public event RouteChangedEventHandler? RouteChanged;

        public List<RouteSwitchDescriptor> CurrentRoutes
        {
            get
            {
                lock (@lock)
                {
                    return [.. currentRoutes];
                }
            }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; } = new();

        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; } = new();

        public void Route(string inputSlotKey, string outputSlotKey, eRoutingSignalType type)
        {
            if (!inputs.TryGetValue(inputSlotKey, out var inputSlot))
            {
                this.LogError("Invalid input slot {inputSlotKey}", inputSlotKey);
                return;
            }

            if (!outputs.TryGetValue(outputSlotKey, out var outputSlot))
            {
                this.LogError("Invalid output slot {outputSlotKey}", outputSlotKey);
                return;
            }

            this.LogInformation("Routing {signalType} from {inputSlot} to {outputSlot}", type, inputSlotKey, outputSlotKey);
            ExecuteSwitch(inputSlot.Transmitter, outputSlot.Receiver, type);
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            if (signalType.HasFlag(eRoutingSignalType.Video) && inputSelector is INvxDevice videoTx && outputSelector is INvxDevice videoRx)
            {
                this.LogInformation("Setting incoming stream url for {videoRx} to {streamUrl}", videoRx.Key, videoTx.StreamUrlFeedback.StringValue);
                videoRx.SetIncomingStreamUrl(videoTx.StreamUrlFeedback.StringValue);
            }

            if (signalType.HasFlag(eRoutingSignalType.Audio) && inputSelector is INvxDevice audioTx && outputSelector is INvxDevice audioRx)
            {
                this.LogInformation("Setting incoming DmNax stream address for {audioRx} to {dmNaxTxAddress}", audioRx.Key, audioTx.DmNaxTxAddressFeedback.StringValue);
                audioRx.SetIncomingDmNaxStreamAddress(audioTx.DmNaxTxAddressFeedback.StringValue);
            }
        }

        class NvxMatrixRoutingInput(
            INvxDevice transmitter,
            RoutingInputPort matrixMixerStreamInputPort,
            RoutingInputPort matrixMixerDmNaxInputPort)
        {
            public readonly INvxDevice Transmitter = transmitter;
            public readonly RoutingInputPort MatrixMixerStreamInputPort = matrixMixerStreamInputPort;
            public readonly RoutingInputPort MatrixMixerDmNaxInputPort = matrixMixerDmNaxInputPort;
            public readonly NvxRoutingInputSlot RoutingInputSlot = new NvxRoutingInputSlot(transmitter);
        }

        class NvxMatrixRoutingOutput(
            INvxDevice receiver,
            RoutingOutputPort matrixMixerStreamOutputPort,
            RoutingOutputPort matrixMixerDmNaxOutputPort)
        {
            public readonly INvxDevice Receiver = receiver;
            public readonly RoutingOutputPort MatrixMixerStreamOutputPort = matrixMixerStreamOutputPort;
            public readonly RoutingOutputPort MatrixMixerDmNaxOutputPort = matrixMixerDmNaxOutputPort;
            public readonly NvxRoutingOutputSlot RoutingOutputSlot = new NvxRoutingOutputSlot(receiver);
        }

        class NvxRoutingInputSlot : IRoutingInputSlot
        {
            private readonly RoutingInputPortWithVideoStatuses? videoSyncInput;

            public readonly INvxDevice Transmitter;

            public NvxRoutingInputSlot(INvxDevice transmitter)
            {
                Transmitter = transmitter;
                videoSyncInput = Transmitter.InputPorts[NvxDevice.Hdmi1RoutingPortKey] as RoutingInputPortWithVideoStatuses;

                if (videoSyncInput != null)
                {
                    videoSyncInput.VideoStatus.VideoSyncFeedback.OutputChange += (s, e) => VideoSyncChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public string TxDeviceKey => Transmitter.Key;

            public int SlotNumber => Transmitter.DeviceId;

            public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo;

            public string Name => Transmitter.Name;

            public BoolFeedback IsOnline => Transmitter is ICommunicationMonitor device
                ? device.CommunicationMonitor.IsOnlineFeedback
                : new BoolFeedback("IsOnline", () => false);

            public bool VideoSyncDetected => videoSyncInput?.VideoStatus.VideoSyncFeedback.BoolValue ?? false;

            public string Key => Transmitter.Key;

            public event EventHandler? VideoSyncChanged;
        }

        class NvxRoutingOutputSlot : IRoutingOutputSlot
        {
            private readonly object @lock = new();

            private readonly Dictionary<eRoutingSignalType, IRoutingInputSlot> currentRoutes;

            public readonly INvxDevice Receiver;

            public NvxRoutingOutputSlot(INvxDevice receiver)
            {
                currentRoutes = new();
                Receiver = receiver;

                receiver.StreamUrlFeedback.OutputChange += (s, e) =>
                {
                    lock (@lock)
                    {
                        var inputSlot = inputs.Values.FirstOrDefault(t => t.Transmitter.StreamUrlFeedback.StringValue == e.StringValue);
                        if (inputSlot == null)
                        {
                            var _ = currentRoutes.Remove(eRoutingSignalType.Video);
                        }
                        else
                        {
                            currentRoutes[eRoutingSignalType.Video] = inputSlot.RoutingInputSlot;
                            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updated video route to {inputSlot}", this, inputSlot.Transmitter.Key);
                        }

                        OutputSlotChanged?.Invoke(this, EventArgs.Empty);
                    }
                };

                receiver.DmNaxRxAddressFeedback.OutputChange += (s, e) =>
                {
                    lock (@lock)
                    {

                        var inputSlot = inputs.Values.FirstOrDefault(t => t.Transmitter.DmNaxTxAddressFeedback.StringValue == e.StringValue);
                        if (inputSlot == null)
                        {
                            var _ = currentRoutes.Remove(eRoutingSignalType.Audio);
                        }
                        else
                        {
                            currentRoutes[eRoutingSignalType.Audio] = inputSlot.RoutingInputSlot;
                            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updated audio route to {inputSlot}", this, inputSlot.Transmitter.Key);
                        }

                        OutputSlotChanged?.Invoke(this, EventArgs.Empty);
                    }
                };
            }

            public string RxDeviceKey => Receiver.Key;

            public int SlotNumber => Receiver.DeviceId;

            public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo;

            public string Name => Receiver.Name;

            public BoolFeedback IsOnline => Receiver is ICommunicationMonitor device
                ? device.CommunicationMonitor.IsOnlineFeedback
                : new BoolFeedback("IsOnline", () => false);

            public string Key => Receiver.Key;

            public Dictionary<eRoutingSignalType, IRoutingInputSlot> CurrentRoutes
            {
                get
                {
                    lock (@lock)
                    {
                        return currentRoutes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    }
                }
            }

            public event EventHandler? OutputSlotChanged;
        }
    }
}
