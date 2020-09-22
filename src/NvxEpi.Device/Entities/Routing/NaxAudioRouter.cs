using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Extensions;
using NvxEpi.Abstractions.NaxAudio;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Routing
{
    public class NaxAudioRouter : EssentialsDevice, IRouting
    {
        public RoutingInputPort Off
        {
            get
            {
                return new RoutingInputPort("$Off", eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, null, this);
            }
        }

        public NaxAudioRouter(string key) : base(key)
        {
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

            AddPreActivationAction(() => DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .ToList()
                .ForEach(tx =>
                {
                    var naxTx = tx as INaxAudioTx;
                    if (naxTx == null)
                        return;

                    var streamRoutingPort = tx.OutputPorts[DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        GetInputPortKeyForTx(naxTx),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        naxTx,
                        this);

                    InputPorts.Add(input);
                }));

            AddPreActivationAction(() => DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .ToList()
                .ForEach(rx =>
                {
                    var naxTx = rx as INaxAudioRx;
                    if (naxTx == null)
                        return;

                    var streamRoutingPort = rx.InputPorts[DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(naxTx),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        naxTx,
                        this);

                    OutputPorts.Add(output);
                }));
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, "Error executing route : '{0}'", ex.Message);
                throw;
            }
        }

        public static void Route(int rxVirtualId, int txVirtualId)
        {
            throw new NotImplementedException();

            if (rxVirtualId == 0)
                return;

            var rx = DeviceManager
                    .AllDevices
                    .OfType<INaxAudioRx>()
                    .FirstOrDefault(x => x.DeviceId == rxVirtualId);

            if (rx == null) return;

            if (txVirtualId == 0)
                rx.StopAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<INaxAudioTx>()
                    .FirstOrDefault(x => x.DeviceId == txVirtualId);

                if (tx == null)
                    return;

                //rx.RouteAudioStream(tx);
            }
        }

        public static void RouteAudioStream(string txName, INaxAudioRx rx)
        {
            throw new NotImplementedException();

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxDeviceRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopAudioStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<INaxAudioTx>()
                .Where(t => t.IsTransmitter)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                //rx.RouteAudioStream(txByName);
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null)
                return;

            //rx.RouteAudioStream(txByKey);
        }

        public static string GetInputPortKeyForTx(INaxAudioTx tx)
        {
            return tx.Key + "--" + "NaxAudio";
        }

        public static string GetOutputPortKeyForRx(INaxAudioRx rx)
        {
            return rx.Key + "--" + "NaxAudio";
        }
    }
}