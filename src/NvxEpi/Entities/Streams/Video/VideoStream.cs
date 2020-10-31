using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Video
{
    public class VideoStream : IStream
    {
        private const string _noRouteAddress = "0.0.0.0";
        private readonly ICurrentVideoInput _device;

        public VideoStream(ICurrentVideoInput device)
        {
            _device = device;
            Initialize();
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public StringFeedback CurrentVideoInput
        {
            get { return _device.CurrentVideoInput; }
        }

        public IntFeedback CurrentVideoInputValue
        {
            get { return _device.CurrentVideoInputValue; }
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

        public BoolFeedback IsStreamingVideo { get; private set; }

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

        public StringFeedback StreamUrl { get; private set; }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback VideoStreamStatus { get; private set; }

        public void ClearStream()
        {
            if (IsTransmitter)
                return;

            Debug.Console(1, this, "Clearing stream");
            Hardware.Control.ServerUrl.StringValue = _noRouteAddress;
        }

        public void SetStreamUrl(string url)
        {
            if (IsTransmitter)
                return;

            if (String.IsNullOrEmpty(url))
                return;

            Debug.Console(1, this, "Setting stream: '{0}", url);
            Hardware.Control.ServerUrl.StringValue = url;
            Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }

        public void SetVideoInput(ushort input)
        {
            _device.SetVideoInput(input);
        }

        public void SetVideoToHdmiInput1()
        {
            _device.SetVideoToHdmiInput1();
        }

        public void SetVideoToHdmiInput2()
        {
            _device.SetVideoToHdmiInput2();
        }

        public void SetVideoToNone()
        {
            _device.SetVideoToNone();
        }

        public void SetVideoToStream()
        {
            _device.SetVideoToStream();
        }

        private void Initialize()
        {
            StreamUrl = StreamUrlFeedback.GetFeedback(Hardware);

            IsStreamingVideo = IsTransmitter
                ? IsStreamingVideoFeedback.GetFeedbackForTx(Hardware)
                : IsStreamingVideoFeedback.GetFeedbackForRx(Hardware);

            VideoStreamStatus = VideoStreamStatusFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
                {
                    StreamUrl,
                    IsStreamingVideo,
                    VideoStreamStatus
                });
        }
    }
}