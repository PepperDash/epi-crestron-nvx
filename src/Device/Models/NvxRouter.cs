using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models
{
    public class NvxRouter : EssentialsDevice, IRouting
    {
        private static NvxRouter _instance;
        private static readonly CCriticalSection _lock = new CCriticalSection();

        public const string RouteOff = "$off";

        public string NoSourceText { get; private set; }

        public static NvxRouter Instance
        {
            get
            {
                _lock.Enter();
                try
                {
                    return _instance ?? (_instance = new NvxRouter("NvxRouterInstance"));
                }
                finally
                {
                    _lock.Leave();
                }
            }
        }

        private NvxRouter(string key) : base(key)
        {
            NoSourceText = "No Source";

            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

            AddPostActivationAction(() => DeviceManager
                .AllDevices
                .OfType<NvxDevice>()
                .Where(x => x.IsTransmitter)
                .ToList()
                .ForEach(tx =>
                {
                    var streamRoutingPort = tx.OutputPorts[VideoOutputEnum.Stream.Name];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        tx.Key + streamRoutingPort.ParentDevice.Key,
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        tx.Key,
                        this);

                    InputPorts.Add(input);
                    TieLineCollection.Default.Add(new TieLine(streamRoutingPort, input));
                }));

            AddPostActivationAction(() => DeviceManager
                .AllDevices
                .OfType<NvxDevice>()
                .Where(x => !x.IsTransmitter)
                .ToList()
                .ForEach(rx =>
                {
                    var streamRoutingPort = rx.InputPorts[VideoInputEnum.Stream.Name];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        rx.Key + streamRoutingPort.ParentDevice.Key,
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        rx.Key,
                        this);

                    OutputPorts.Add(output);
                    TieLineCollection.Default.Add(new TieLine(output, streamRoutingPort));
                }));
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var txKey = inputSelector as string;
            var rx = outputSelector as NvxDevice;
            if (rx == null)
                return;

            RouteVideo(rx, String.IsNullOrEmpty(txKey) ? RouteOff : txKey);
        }

        public static void RouteVideo(NvxDevice rx, string txName)
        {
            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopVideoStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<NvxDevice>()
                .Where(t => t.IsTransmitter)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                RouteVideo(rx, txByName);
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null)
                return;

            RouteVideo(rx, txByKey);
        }

        public static void RouteVideo(NvxDevice rx, NvxDevice tx)
        {
            if (tx == null)
                throw new NullReferenceException("tx");

            if (rx == null)
                throw new NullReferenceException("rx");

            rx.SetStreamUrl(tx.StreamUrl);
            rx.SetVideoInput((ushort) eSfpVideoSourceTypes.Stream);
            rx.StartVideoStream();
        }

        public static void RouteAudio(NvxDevice rx, string txName)
        {
            if (String.IsNullOrEmpty(txName))
                return;

            rx.SetAudioInput((ushort) DmNvxControl.eAudioSource.SecondaryStreamAudio);
            if (txName.Equals(RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopAudioStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<NvxDevice>()
                .Where(t => t.IsTransmitter)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                RouteAudio(rx, txByName);
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null)
                return;

            RouteAudio(rx, txByKey);
        }

        public static void RouteAudio(NvxDevice rx, NvxDevice tx)
        {
            if (tx == null)
                throw new NullReferenceException("tx");

            if (rx == null)
                throw new NullReferenceException("rx");

            rx.SetAudioMulticastAddress(
                String.IsNullOrEmpty(tx.MulticastAudioAddress)
                    ? tx.MulticastAddress
                    : tx.MulticastAudioAddress);

            rx.StartAudioStream();
        }
    }
}