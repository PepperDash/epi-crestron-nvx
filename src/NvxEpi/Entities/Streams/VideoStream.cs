using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public class VideoStream : IStream
    {
        private readonly INvxHardware _device;

        public VideoStream(INvxHardware device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            StreamUrl = StreamUrlFeedback.GetFeedback(Hardware);
            IsStreamingVideo = IsStreamingVideoFeedback.GetFeedback(Hardware);
            VideoStreamStatus = VideoStreamStatusFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
            {
                StreamUrl,
                IsStreamingVideo,
                VideoStreamStatus
            });
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public StringFeedback StreamUrl { get; private set; }
        public BoolFeedback IsStreamingVideo { get; private set; }
        public StringFeedback VideoStreamStatus { get; private set; }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }
    }
}