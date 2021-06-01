using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing
{
    public class PrimaryStreamRouter : EssentialsDevice, IRouting
    {
        public PrimaryStreamRouter(string key) : base(key)
        {
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                if (signalType.Is(eRoutingSignalType.Audio))
                    throw new ArgumentException("signal type must include video");

                var rx = outputSelector as IStreamWithHardware;
                if (rx == null)
                    throw new ArgumentNullException("rx");

                var tx = inputSelector as IStreamWithHardware;
                if (tx == null)
                {
                    rx.ClearStream();
                    return;
                }
                    
                rx.RouteStream(tx);
                if (!signalType.Has(eRoutingSignalType.Audio)) 
                    return;

                var audioInputSwitcher = rx as ICurrentAudioInput;
                if (audioInputSwitcher == null)
                    return;

                audioInputSwitcher.SetAudioToPrimaryStreamAudio();
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, "Error executing route : '{0}'", ex.Message);
            }
        }

        public override bool CustomActivate()
        {

            if (_transmitters == null)
                _transmitters = GetTransmitterDictionary();

            if (_receivers == null)
                _receivers = GetReceiverDictionary();

            _transmitters
                .Values
                .ToList()
                .ForEach(tx =>
                    {
                        var streamRoutingPort = tx.OutputPorts[SwitcherForStreamOutput.Key];
                        if (streamRoutingPort == null)
                            return;

                        var input = new RoutingInputPort(
                            GetInputPortKeyForTx(tx),
                            eRoutingSignalType.AudioVideo,
                            eRoutingPortConnectionType.Streaming,
                            tx,
                            this);

                        InputPorts.Add(input);
                    });

            _receivers
                .Values
                .ToList()
                .ForEach(rx =>
                    {
                        var streamRoutingPort = rx.InputPorts[DeviceInputEnum.Stream.Name];
                        if (streamRoutingPort == null)
                            return;

                        var output = new RoutingOutputPort(
                            GetOutputPortKeyForRx(rx),
                            eRoutingSignalType.AudioVideo,
                            eRoutingPortConnectionType.Streaming,
                            rx,
                            this);

                        OutputPorts.Add(output);
                    });

            foreach (var routingOutputPort in OutputPorts)
            {
                var port = routingOutputPort;
                port.InUseTracker.InUseFeedback.OutputChange += (sender, args) =>
                {
                    if (args.BoolValue)
                        return;

                    ExecuteSwitch(null, port.Selector, eRoutingSignalType.AudioVideo);
                };
            }

            return base.CustomActivate();
        }

        public static string GetInputPortKeyForTx(IStream tx)
        {
            return tx.Key + "-Stream";
        }

        public static string GetOutputPortKeyForRx(IStream rx)
        {
            return rx.Key + "-StreamOutput";
        }

        public static void Route(int txId, int rxId)
        {
            if (rxId == 0)
                return;

            var rx = _receivers.Values.FirstOrDefault(x => x.DeviceId == rxId);
            if (rx == null)
                return;

            Route(txId, rx);
        }

        public static void Route(int txId, IStream rx)
        {
            if (rx.IsTransmitter)
                throw new ArgumentException("rx device is transmitter");

            if (txId == 0)
            {
                rx.ClearStream();
                return;
            }

            var tx = _receivers.Values.FirstOrDefault(x => x.DeviceId == txId);
            if (tx == null)
                return;

            rx.RouteStream(tx);
        }

        public static void Route(string txName, IStream rx)
        {
            if (rx.IsTransmitter)
                throw new ArgumentException("rx device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxGlobalRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.ClearStream();
                return;
            }

            IStream txByName;
            if (_transmitters.TryGetValue(txName, out txByName))
            {
                rx.RouteStream(txByName);
                return;
            }

            var txByKey = _transmitters
                .Values
                .FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));

            if (txByKey == null)
                return;

            rx.RouteStream(txByKey);
        }

        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static Dictionary<string, IStream> _receivers;
        private static Dictionary<string, IStream> _transmitters;

        private static Dictionary<string, IStream> GetTransmitterDictionary()
        {
            try
            {
                _lock.Enter();
                var dict = new Dictionary<string, IStream>(StringComparer.OrdinalIgnoreCase);

                foreach (var device in DeviceManager.AllDevices.OfType<IStream>())
                {
                    if (!device.IsTransmitter)
                        continue;

                    dict.Add(device.Name, device);
                }

                return dict;
            }
            finally
            {
                _lock.Leave();
            }
        }

        private static Dictionary<string, IStream> GetReceiverDictionary()
        {
            try
            {
                _lock.Enter();

                var dict = new Dictionary<string, IStream>(StringComparer.OrdinalIgnoreCase);
                foreach (var device in DeviceManager.AllDevices.OfType<IStream>())
                {
                    if (device.IsTransmitter)
                        continue;

                    dict.Add(device.Name, device);
                }

                return dict;
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}