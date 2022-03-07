using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing
{
    public class SecondaryAudioRouter : EssentialsDevice, IRouting
    {
        public SecondaryAudioRouter(string key) : base(key)
        {
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();
        }

        public override bool CustomActivate()
        {
            if (_transmitters == null)
                _transmitters = GetTransmitterDictionary();

            if (_receivers == null)
                _receivers = GetReceiverDictionary();

            new [] {_transmitters.Values, _receivers.Values}
                .SelectMany(x => x)
                .ToList()
                .ForEach(device =>
                    {
                        var streamInputPort = device.OutputPorts[SwitcherForSecondaryAudioOutput.Key];
                        if (streamInputPort != null)
                        {
                            var input = new RoutingInputPort(
                                GetInputPortKeyForTx(device),
                                eRoutingSignalType.Audio,
                                eRoutingPortConnectionType.Streaming,
                                device,
                                this);

                            InputPorts.Add(input);
                        }

                        var streamOutputPort = device.InputPorts[DeviceInputEnum.SecondaryAudio.Name];
                        if (streamOutputPort == null)
                            return;

                        var output = new RoutingOutputPort(
                            GetOutputPortKeyForRx(device),
                            eRoutingSignalType.Audio,
                            eRoutingPortConnectionType.Streaming,
                            device,
                            this);

                        OutputPorts.Add(output);
                    });

            foreach (var routingOutputPort in OutputPorts)
            {
                var port = routingOutputPort;
                const int delayTime = 250;

                var timer = new CTimer(o =>
                {
                    if (port.InUseTracker.InUseFeedback.BoolValue)
                        return;

                    ExecuteSwitch(null, port.Selector, eRoutingSignalType.Audio);
                }, Timeout.Infinite);

                port.InUseTracker.InUseFeedback.OutputChange += (sender, args) =>
                {
                    if (args.BoolValue)
                        return;

                    timer.Reset(delayTime);
                };
            }

            return base.CustomActivate();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying execute switch secondary audio route: {0} {1}", inputSelector, outputSelector);
                if (!signalType.Has(eRoutingSignalType.Audio))
                    throw new ArgumentException("signal type must include audio");

                var rx = outputSelector as ISecondaryAudioStream;
                if (rx == null)
                    throw new ArgumentNullException("rx");

                rx.RouteSecondaryAudio(inputSelector as ISecondaryAudioStream);
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, "Error executing route : '{0}'", ex.Message);
                throw;
            }
        }

        public static string GetInputPortKeyForTx(ISecondaryAudioStream tx)
        {
            return tx.Key + "-SecondaryAudio";
        }

        public static string GetOutputPortKeyForRx(ISecondaryAudioStream rx)
        {
            return rx.Key + "-SecondaryAudioOutput";
        }

        public static void Route(int txId, int rxId)
        {
            Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying secondary audio route by txId & rxId: {0} {1}", txId, rxId);
            if (rxId == 0)
                return;

            var rx = _receivers.Values.FirstOrDefault(x => x.DeviceId == rxId);
            if (rx == null)
                return;

            if (txId == 0)
            {
                rx.ClearSecondaryStream();
                return;
            }

            Route(txId, rx);
        }

        public static void Route(int txId, ISecondaryAudioStream rx)
        {
            Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying secondary audio route by txId & address: {0} {1}", txId, rx.RxAudioAddress);
            if (txId == 0)
            {
                rx.ClearSecondaryStream();
                return;
            }

            var tx = _transmitters.Values.FirstOrDefault(x => x.DeviceId == txId);
            if (tx == null)
                return;

            rx.RouteSecondaryAudio(tx);
        }

        public static void Route(string txName, ISecondaryAudioStream rx)
        {
            Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying secondary audio route by txName & address: {0} {1}", txName, rx.RxAudioAddress);
            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxGlobalRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.ClearSecondaryStream();
                return;
            }

            ISecondaryAudioStream txByName;
            if (_transmitters.TryGetValue(txName, out txByName))
            {
                rx.RouteSecondaryAudio(txByName);
                return;
            }

            var txByKey = _transmitters
                .Values
                .FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));

            if (txByKey == null)
                return;

            rx.RouteSecondaryAudio(txByKey);
        }

        private static Dictionary<string, ISecondaryAudioStream> _transmitters;
        private static Dictionary<string, ISecondaryAudioStream> _receivers;

        private static Dictionary<string, ISecondaryAudioStream> GetTransmitterDictionary()
        {
            return
                DeviceManager
                    .AllDevices
                    .OfType<ISecondaryAudioStream>()
                    .Where(device => device.IsTransmitter)
                    .ToDictionary(device => device.Name, stream => stream);
        }

        private static Dictionary<string, ISecondaryAudioStream> GetReceiverDictionary()
        {
            return
                DeviceManager
                    .AllDevices
                    .OfType<ISecondaryAudioStream>()
                    .Where(device => !device.IsTransmitter)
                    .ToDictionary(device => device.Name, stream => stream);
        }
    }
}