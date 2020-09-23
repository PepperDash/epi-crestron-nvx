using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Config;
using NvxEpi.Device.Entities.InputSwitching;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.Services.Feedback;
using NvxEpi.Device.Services.Utilities;
using NvxEpi.Extensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Entities.Hardware
{
    public abstract class NvxBaseHardware : IStream, ICurrentVideoInput, ICurrentAudioInput
    {
        protected readonly IStream _stream;
        protected readonly ICurrentVideoInput _videoSwitcher;
        protected readonly ICurrentAudioInput _audioSwitcher;

        protected NvxBaseHardware(DeviceConfig config, DmNvxBaseClass hardware, FeedbackCollection<Feedback> feedbacks, BoolFeedback isOnline)
        {
            IsOnline = isOnline;
            Feedbacks = feedbacks;
            Key = config.Key;
            Name = config.Name;
            Hardware = hardware;
            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

            var props = NvxDeviceProperties.FromDeviceConfig(config);
            DeviceId = props.DeviceId;
            IsTransmitter = props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);

            _stream = new VideoStream(this);
            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);

            hardware.OnlineStatusChange += (device, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                Hardware.Control.Name.StringValue = Key.Replace(' ', '-');

                if (IsTransmitter)
                    Hardware.SetTxDefaults(props);
                else
                    Hardware.SetRxDefaults(props);

                Hardware.SetDefaultInputs(props);
            };

            SetupFeedbacks();
        }

        private void SetupFeedbacks()
        {
            DeviceMode = DeviceModeFeedback.GetFeedback(Hardware);
            MulticastAddress = MulticastAddressFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
            {
                DeviceNameFeedback.GetFeedback(Name),
                DeviceMode,
                MulticastAddress,
                _stream.IsStreamingVideo,
                _stream.StreamUrl,
                _stream.VideoStreamStatus,
                _videoSwitcher.CurrentVideoInput,
                _videoSwitcher.CurrentVideoInputValue,
                _audioSwitcher.CurrentAudioInput,
                _audioSwitcher.CurrentAudioInputValue
            });
        }

        public bool IsTransmitter { get; private set; }
        public int DeviceId { get; private set; }
        public IntFeedback DeviceMode { get; private set; }
        public StringFeedback MulticastAddress { get; private set; }

        public DmNvxBaseClass Hardware { get; private set; }

        public StringFeedback StreamUrl
        {
            get { return _stream.StreamUrl; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _stream.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _stream.VideoStreamStatus; }
        }

        public StringFeedback CurrentVideoInput
        {
            get { return _videoSwitcher.CurrentVideoInput; }
        }

        public IntFeedback CurrentVideoInputValue
        {
            get { return _videoSwitcher.CurrentVideoInputValue; }
        }

        public StringFeedback CurrentAudioInput
        {
            get { return _audioSwitcher.CurrentAudioInput; }
        }

        public IntFeedback CurrentAudioInputValue
        {
            get { return _audioSwitcher.CurrentAudioInputValue; }
        }

        public string Key { get; private set; }
        public string Name { get; private set; }
        public FeedbackCollection<Feedback> Feedbacks { get; private set; }
        public BoolFeedback IsOnline { get; private set; }
        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }
    }
}