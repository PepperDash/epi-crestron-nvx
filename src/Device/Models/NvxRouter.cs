using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models.Entities;
using NvxEpi.Device.Services.DeviceExtensions;
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
                .OfType<INvxDevice>()
                .Where(x => x.IsTransmitter.BoolValue)
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
                .Where(x => !x.IsTransmitter.BoolValue)
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
                        rx,
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
            var rx = outputSelector as INvxDevice;
            if (rx == null)
                return;

            rx.RouteVideoStream(String.IsNullOrEmpty(txKey) ? RouteOff : txKey);
        }

        
    }
}