using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Aggregates
{
    public abstract class NvxBaseDevice : CrestronGenericBridgeableBaseDevice, ICurrentVideoInput, ICurrentAudioInput
    {
        private readonly ICurrentAudioInput _audioSwitcher;
        private readonly ICurrentVideoInput _videoSwitcher;

        protected NvxBaseDevice(DeviceConfig config, DmNvxBaseClass hardware)
            : base(config.Key, config.Name, hardware)
        {
            Hardware = hardware;

            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

            var props = NvxDeviceProperties.FromDeviceConfig(config);
            DeviceId = props.DeviceId;
            IsTransmitter = !String.IsNullOrEmpty(props.Mode) &&
                            props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);

            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);

            SetupFeedbacks(props);
            RegisterForOnlineFeedback(Hardware, props);
        }

        public StringFeedback AudioName { get; private set; }

        public StringFeedback CurrentAudioInput
        {
            get { return _audioSwitcher.CurrentAudioInput; }
        }

        public IntFeedback CurrentAudioInputValue
        {
            get { return _audioSwitcher.CurrentAudioInputValue; }
        }

        public StringFeedback CurrentVideoInput
        {
            get { return _videoSwitcher.CurrentVideoInput; }
        }

        public IntFeedback CurrentVideoInputValue
        {
            get { return _videoSwitcher.CurrentVideoInputValue; }
        }

        public int DeviceId { get; private set; }

        public IntFeedback DeviceMode { get; private set; }

        public new DmNvxBaseClass Hardware { get; private set; }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }

        public bool IsTransmitter { get; private set; }

        public StringFeedback MulticastAddress { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public StringFeedback SecondaryAudioAddress { get; private set; }

        public StringFeedback StreamUrl { get; private set; }

        public StringFeedback VideoName { get; private set; }

        public override string ToString()
        {
            return Key;
        }

        private void RegisterForOnlineFeedback(GenericBase hardware, NvxDeviceProperties props)
        {
            hardware.OnlineStatusChange += (device, args) =>
                {
                    if (!args.DeviceOnLine)
                        return;

                    Hardware.Control.Name.StringValue = Key.Replace(' ', '-');
                };
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
            StreamUrl = StreamUrlFeedback.GetFeedback(Hardware);
            MulticastAddress = MulticastAddressFeedback.GetFeedback(Hardware);
            SecondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
                {
                    DeviceNameFeedback.GetFeedback(Name),
                    StreamUrl,
                    MulticastAddress,
                    VideoName,
                    AudioName,
                    DeviceMode
                });
        }
    }
}