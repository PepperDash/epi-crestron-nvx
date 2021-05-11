using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Video
{
    public class VideoStream : IStreamWithHardware
    {
        private readonly INvxDeviceWithHardware _device;

        private readonly StringFeedback _streamUrl;
        private readonly BoolFeedback _isStreamingVideo;
        private readonly StringFeedback _videoStreamStatus;
        private readonly StringFeedback _multicastVideoAddress;

        public VideoStream(INvxDeviceWithHardware device)
        {
            _device = device;

            _streamUrl = StreamUrlFeedback.GetFeedback(Hardware);
            _isStreamingVideo = IsStreamingVideoFeedback.GetFeedback(Hardware);
            _videoStreamStatus = VideoStreamStatusFeedback.GetFeedback(Hardware);
            _multicastVideoAddress = MulticastAddressFeedback.GetFeedback(Hardware);

            Feedbacks.Add(_multicastVideoAddress);
            Feedbacks.Add(_isStreamingVideo);
            Feedbacks.Add(_videoStreamStatus);
            Feedbacks.Add(_streamUrl);
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
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

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _isStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _videoStreamStatus; }
        }

        public StringFeedback StreamUrl
        {
            get { return _streamUrl; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _multicastVideoAddress; }
        }
    }
}