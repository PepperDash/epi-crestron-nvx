using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Video
{
    public class VideoStream : IStream
    {
        private readonly INvxHardware _device;
        private readonly BoolFeedback _isStreamingVideo;
        private readonly StringFeedback _videoStreamStatus;

        public VideoStream(INvxHardware device)
        {
            _device = device;

            _isStreamingVideo = IsStreamingVideoFeedback.GetFeedback(Hardware);
            _videoStreamStatus = VideoStreamStatusFeedback.GetFeedback(Hardware);

            Feedbacks.Add(_isStreamingVideo);
            Feedbacks.Add(_videoStreamStatus);
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
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

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _device.SecondaryAudioAddress; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _isStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _videoStreamStatus; }
        }
    }
}