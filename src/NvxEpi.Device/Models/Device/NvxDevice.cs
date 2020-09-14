using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Builders;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Models.Entities
{
    public class NvxDevice : INvxDevice
    {
        public DeviceConfig Config { get; private set; }

        public NvxDevice(INvxDeviceBuilder builder)
        {
            VirtualDeviceId = builder.VirtualDeviceId;
            Hardware = builder.Device;
            Config = builder.Config;
            IsTransmitter = builder.IsTransmitter;
            Feedbacks = new FeedbackCollection<Feedback>();
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();
            
            Initialize();
        }

        private void Initialize()
        {
            IsStreamingVideo = Hardware.GetIsStreamingFeedback();
            DeviceName = Hardware.GetDeviceNameFeedback(Config);
            VideoStreamStatus = Hardware.GetDeviceStatusFeedback();
            StreamUrl = Hardware.GetStreamUrlFeedback();
            MulticastAddress = Hardware.GetMulticastAddressFeedback();

            Feedbacks.AddRange(new Feedback[]
            {
                IsTransmitter,
                IsStreamingVideo,
                DeviceName,
                VideoStreamStatus,
                StreamUrl,
                MulticastAddress
            });
        }

        public DmNvxBaseClass Hardware { get; private set; }
        public FeedbackCollection<Feedback> Feedbacks { get; private set; }
        public int VirtualDeviceId { get; private set; }
        public BoolFeedback IsTransmitter { get; private set; }
        public BoolFeedback IsStreamingVideo { get; private set; }
        public StringFeedback DeviceName { get; private set; }
        public StringFeedback VideoStreamStatus { get; private set; }
        public StringFeedback StreamUrl { get; private set; }
        public StringFeedback MulticastAddress { get; private set; }
        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }
        public string Key { get { return Config.Key; } }
        public string Name { get { return Config.Name; } }
        public UsageTracking UsageTracker { get; set; }
    }
}