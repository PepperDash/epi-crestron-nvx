using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Extensions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Routing
{
    public class SecondaryAudioRouter : EssentialsDevice, IRouting
    {
        public RoutingInputPort Off
        {
            get
            {
                return new RoutingInputPort("$Off", eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, null, this);
            }
        }

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
        
                    var streamRoutingPort = tx.OutputPorts[DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString()];
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

                    var streamRoutingPort = rx.InputPorts[DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString()];
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

            AddPreActivationAction(CheckDictionaries);
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
            return tx.Key + "--" + "SecondaryAudio";
        }

        public static string GetOutputPortKeyForRx(ISecondaryAudioStream rx)
        {
            return rx.Key + "--" + "SecondaryAudio";
        }

        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static Dictionary<string, ISecondaryAudioStream> _transmitters;
        private static Dictionary<int, ISecondaryAudioStream> _receivers;

        public static void Route(int txId, int rxId)
        {
            if (rxId == 0)
                return;

            ISecondaryAudioStream rx;
            if (!_receivers.TryGetValue(rxId, out rx))
                return;

            if (txId == 0)
            {
                rx.StopSecondaryAudio();
                return;
            }

            rx.RouteSecondaryAudio(txId);
        }

        public static void Route(string txName, ISecondaryAudioStream rx)
        {
            if (rx.IsTransmitter)
                throw new NotSupportedException("route device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxDeviceRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopSecondaryAudio();
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
                    .ToDictionary(x => x.DeviceId);
            }
            catch (Exception ex)
            {
                Debug.Console(0, NvxDeviceRouter.Instance.SecondaryAudioRouter, "There was an error building the dictionaries: {1}\n{2}", ex.Message, ex.StackTrace);
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}