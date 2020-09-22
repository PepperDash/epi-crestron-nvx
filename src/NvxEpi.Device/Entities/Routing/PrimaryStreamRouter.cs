using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Extensions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Routing
{
    public class PrimaryStreamRouter : EssentialsDevice, IRouting
    {
        public RoutingInputPort Off
        {
            get
            {
                return new RoutingInputPort("$Off", eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, null, this);
            }
        }

        public PrimaryStreamRouter(string key) : base(key)
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
                    var stream = tx as IStream;
                    if (stream == null)
                        return;
                    
                    var streamRoutingPort = tx.OutputPorts[eSfpVideoSourceTypes.Stream.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        GetInputPortKeyForTx(stream),
                        eRoutingSignalType.AudioVideo,
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
                    var stream = rx as IStream;
                    if (stream == null)
                        return;

                    var streamRoutingPort = rx.InputPorts[eSfpVideoSourceTypes.Stream.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(stream),
                        eRoutingSignalType.AudioVideo,
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
                if (signalType.Is(eRoutingSignalType.Audio))
                    throw new ArgumentException("signal type must include video");

                var rx = outputSelector as IStream;
                if (rx != null)
                    rx.RouteStream(inputSelector as IStream);

                if (!signalType.Has(eRoutingSignalType.Audio)) return;

                var rxWithAudioInput = rx as INvx35XHardware;
                if (rxWithAudioInput == null) return;

                rxWithAudioInput.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, "Error executing route : '{0}'", ex.Message);
                throw;
            }
        }

        public static string GetInputPortKeyForTx(IStream tx)
        {
            return tx.Key + "--" + "Stream";
        }

        public static string GetOutputPortKeyForRx(IStream rx)
        {
            return rx.Key + "--" + "Stream";
        }

        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static Dictionary<int, IStream> _receivers;
        private static Dictionary<string, IStream> _transmitters;

        public static void Route(int txDeviceId, int rxDeviceId)
        {
            if (rxDeviceId == 0)
                return;

            IStream rx;
            if (!_receivers.TryGetValue(rxDeviceId, out rx))
                return;

            if (txDeviceId == 0)
                rx.StopStream();
            else
            {
                rx.RouteStream(txDeviceId);
            }
        }

        public static void Route(string txName, IStream rx)
        {
            if (rx.IsTransmitter)
                throw new NotSupportedException("route device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxDeviceRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopStream();
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

        private static void CheckDictionaries()
        {
            try
            {
                _lock.Enter();
                if (_transmitters == null)
                {
                    _transmitters = DeviceManager
                        .AllDevices
                        .OfType<IStream>()
                        .Where(x => x.IsTransmitter)
                        .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
                }

                if (_receivers != null) return;

                _receivers = DeviceManager
                    .AllDevices
                    .OfType<IStream>()
                    .Where(x => !x.IsTransmitter)
                    .ToDictionary(x => x.DeviceId);
            }
            catch (Exception ex)
            {
                Debug.Console(0, NvxDeviceRouter.Instance.PrimaryStreamRouter, "There was an error building the dictionaries: {1}\n{2}", ex.Message, ex.StackTrace);
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}