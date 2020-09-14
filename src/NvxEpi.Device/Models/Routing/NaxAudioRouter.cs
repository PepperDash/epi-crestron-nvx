using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Aggregates;
using NvxEpi.Device.Services.DeviceExtensions;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Routing
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
                .OfType<IHasNaxAudioTxStream>()
                .ToList()
                .ForEach(tx =>
                {
                    var streamRoutingPort = tx.OutputPorts[DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        GetInputPortKeyForTx(tx),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        tx,
                        this);

                    InputPorts.Add(input);
                }));

            AddPreActivationAction(() => DeviceManager
                .AllDevices
                .OfType<IHasNaxAudioRxStream>()
                .ToList()
                .ForEach(rx =>
                {
                    var streamRoutingPort = rx.InputPorts[DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(rx),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        rx,
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
                if (signalType.Has(eRoutingSignalType.Video))
                    throw new ArgumentException("signal type cannot include video");

                if (!signalType.Has(eRoutingSignalType.Audio))
                    throw new ArgumentException("signal type must include audio");

                var rx = outputSelector as IHasHasNaxAudioStreamRouting;
                if (rx == null)
                    throw new ArgumentNullException("rx");

                rx.RouteAudioStream(inputSelector as IHasHasNaxAudioStreamRouting);
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, "Error executing route : '{0}'", ex.Message);
                throw;
            }
        }

        public static void Route(int rxVirtualId, int txVirtualId)
        {
            if (rxVirtualId == 0)
                return;

            var rx = DeviceManager
                    .AllDevices
                    .OfType<IHasHasNaxAudioStreamRouting>()
                    .FirstOrDefault(x => x.VirtualDeviceId == rxVirtualId);

            if (rx == null) return;

            if (txVirtualId == 0)
                rx.StopAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IHasNaxAudioTxStream>()
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteAudioStream(tx);
            }
        }

        public static string GetInputPortKeyForTx(IHasNaxAudioTxStream tx)
        {
            return tx.Key + "--" + "NaxAudio";
        }

        public static string GetOutputPortKeyForRx(IHasNaxAudioRxStream rx)
        {
            return rx.Key + "--" + "NaxAudio";
        }
    }
}