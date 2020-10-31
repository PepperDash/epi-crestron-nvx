using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Routing
{
    public class SecondaryAudioRouter : EssentialsDevice, IRouting
    {
        public SecondaryAudioRouter(string key) : base(key)
        {
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

            AddPreActivationAction(() => DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(x => x.IsTransmitter)
                .ToList()
                .ForEach(tx =>
                {
                    var stream = tx as ISecondaryAudioStream;
                    if (stream == null)
                        return;
        
                    var streamRoutingPort = tx.OutputPorts[SecondaryAudioOutput.Key];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        GetInputPortKeyForTx(stream),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        stream,
                        this);

                    InputPorts.Add(input);
                }));

            AddPreActivationAction(() => DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(x => !x.IsTransmitter)
                .ToList()
                .ForEach(rx =>
                {
                    var stream = rx as ISecondaryAudioStream;
                    if (stream == null)
                        return;

                    var streamRoutingPort = rx.InputPorts[DeviceInputEnum.SecondaryAudio.Name];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(stream),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        stream,
                        this);

                    OutputPorts.Add(output);
                }));
        }

        public override bool CustomActivate()
        {
            foreach (var routingOutputPort in OutputPorts)
            {
                var port = routingOutputPort;
                port.InUseTracker.InUseFeedback.OutputChange += (sender, args) =>
                {
                    if (args.BoolValue)
                        return;

                    ExecuteSwitch(null, port.Selector, eRoutingSignalType.Audio);
                };
            }

            CheckDictionaries();
            return base.CustomActivate();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                if (signalType.Has(eRoutingSignalType.Video))
                    throw new ArgumentException("signal type cannot include video");

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
            return "SecondaryAudio" + "--" + tx.Key;
        }

        public static string GetOutputPortKeyForRx(ISecondaryAudioStream rx)
        {
            return "SecondaryAudio" + "--" + rx.Key;
        }

        public static void Route(int txId, int rxId)
        {
            if (rxId == 0)
                return;

            var rx = _receivers.Values.FirstOrDefault(x => x.DeviceId == rxId);
            if (rx == null)
                return;

            if (txId == 0)
            {
                rx.ClearSecondaryAudioStream();
                return;
            }

            Route(txId, rx);
        }

        public static void Route(int txId, ISecondaryAudioStream rx)
        {
            if (rx.IsTransmitter)
                throw new ArgumentException("rx device is transmitter");

            if (txId == 0)
            {
                rx.ClearSecondaryAudioStream();
                return;
            }

            var tx = _receivers.Values.FirstOrDefault(x => x.DeviceId == txId);
            if (tx == null)
                return;

            rx.RouteSecondaryAudio(tx);
        }

        public static void Route(string txName, ISecondaryAudioStream rx)
        {
            if (rx.IsTransmitter)
                throw new ArgumentException("rx device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxGlobalRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.ClearSecondaryAudioStream();
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

        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static Dictionary<string, ISecondaryAudioStream> _transmitters;
        private static Dictionary<string, ISecondaryAudioStream> _receivers;

        private static void CheckDictionaries()
        {
            try
            {
                _lock.Enter();
                if (_transmitters == null)
                {
                    _transmitters = DeviceManager
                        .AllDevices
                        .OfType<ISecondaryAudioStream>()
                        .Where(x => x.IsTransmitter)
                        .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
                }

                if (_receivers != null) return;

                _receivers = DeviceManager
                    .AllDevices
                    .OfType<ISecondaryAudioStream>()
                    .Where(x => x.IsTransmitter)
                    .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Debug.Console(0, NvxGlobalRouter.Instance.SecondaryAudioRouter, "There was an error building the dictionaries: {1}\n{2}", ex.Message, ex.StackTrace);
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}