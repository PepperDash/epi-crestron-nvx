using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Entities.Hardware
{
    public abstract class NvxBaseHardware : ICurrentVideoInput, ICurrentAudioInput
    {
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
                
            SetupFeedbacks(props);
            IsTransmitter = props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);

            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);
        }

        private void SetupFeedbacks(NvxDeviceProperties props)
        {
            VideoName = String.IsNullOrEmpty(props.VideoSourceName)
                ? new StringFeedback("VideoName", () => Name)
                : new StringFeedback("VideoName", () => props.VideoSourceName);
 
            AudioName = String.IsNullOrEmpty(props.AudioSourceName)
                ? new StringFeedback("AudioName", () => Name)
                : new StringFeedback("AudioName", () => props.AudioSourceName);

            DeviceMode = DeviceModeFeedback.GetFeedback(Hardware);
            MulticastAddress = MulticastAddressFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
            {
                DeviceNameFeedback.GetFeedback(Name),
                VideoName,
                AudioName,
                DeviceMode
            });
        }

        public bool IsTransmitter { get; protected set; }
        public int DeviceId { get; private set; }

        public IntFeedback DeviceMode { get; private set; }
        public StringFeedback MulticastAddress { get; private set; }

        public DmNvxBaseClass Hardware { get; private set; }

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
        public StringFeedback VideoName { get; private set; }
        public StringFeedback AudioName { get; private set; }
    }
}