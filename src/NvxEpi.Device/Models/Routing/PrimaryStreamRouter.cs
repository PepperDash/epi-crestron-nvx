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
                .OfType<IHasVideoStreamRouting>()
                .Where(x => x.IsTransmitter.BoolValue)
                .ToList()
                .ForEach(tx =>
                {
                    var streamRoutingPort = tx.OutputPorts[eSfpVideoSourceTypes.Stream.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        GetInputPortKeyForTx(tx),
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        tx,
                        this);

                    InputPorts.Add(input);
                }));

            AddPreActivationAction(() => DeviceManager
                .AllDevices
                .OfType<IHasVideoStreamRouting>()
                .Where(x => !x.IsTransmitter.BoolValue)
                .ToList()
                .ForEach(rx =>
                {
                    var streamRoutingPort = rx.InputPorts[eSfpVideoSourceTypes.Stream.ToString()];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(rx),
                        eRoutingSignalType.AudioVideo,
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
                if (signalType.Is(eRoutingSignalType.Audio))
                    throw new ArgumentException("signal type must include video");

                var rx = outputSelector as IHasVideoStreamRouting;
                if (rx != null) 
                    rx.RouteVideoStream(inputSelector as IHasVideoStreamRouting);

                if (!signalType.Has(eRoutingSignalType.Audio)) return;

                var rxWithAudioInput = rx as INvx35x;
                if (rxWithAudioInput != null)
                    rxWithAudioInput.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
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
                    .OfType<IHasVideoStreamRouting>()
                    .Where(x => !x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == rxVirtualId);

            if (rx == null) return;

            if (txVirtualId == 0)
                rx.StopVideoStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IHasVideoStreamRouting>()
                    .Where(x => x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteVideoStream(tx);
            }
        }

        public static string GetInputPortKeyForTx(IHasVideoStreamRouting tx)
        {
            return tx.Key + "--" + "Stream";
        }

        public static string GetOutputPortKeyForRx(IHasVideoStreamRouting rx)
        {
            return rx.Key + "--" + "Stream";
        }
    }
}