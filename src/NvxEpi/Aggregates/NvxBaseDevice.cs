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
        private readonly bool _isTransmitter;
        private readonly int _deviceId;

        private readonly DmNvxBaseClass _hardware;

        private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
            new RoutingPortCollection<RoutingInputPort>();

        private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
            new RoutingPortCollection<RoutingOutputPort>();

        private readonly IntFeedback _deviceMode;

        private readonly StringFeedback _audioName;
        private readonly StringFeedback _multicastAddress;
        private readonly StringFeedback _secondaryAudioAddress;
        private readonly StringFeedback _streamUrl;
        private readonly StringFeedback _videoName;

        private readonly ICurrentVideoInput _videoSwitcher;
        private readonly ICurrentAudioInput _audioSwitcher;

        protected NvxBaseDevice(DeviceConfig config, DmNvxBaseClass hardware)
            : base(config.Key, config.Name, hardware)
        {
            _hardware = hardware;

            var props = NvxDeviceProperties.FromDeviceConfig(config);
            _deviceId = props.DeviceId;
            _isTransmitter = !String.IsNullOrEmpty(props.Mode) &&
                             props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);

            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);

            _videoName = String.IsNullOrEmpty(props.VideoSourceName)
                ? new StringFeedback("VideoName", () => Name)
                : new StringFeedback("VideoName", () => props.VideoSourceName);

            _audioName = String.IsNullOrEmpty(props.AudioSourceName)
                ? new StringFeedback("AudioName", () => Name)
                : new StringFeedback("AudioName", () => props.AudioSourceName);

            _deviceMode = DeviceModeFeedback.GetFeedback(Hardware);
            _streamUrl = StreamUrlFeedback.GetFeedback(Hardware);
            _multicastAddress = MulticastAddressFeedback.GetFeedback(Hardware);
            _secondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
                {
                    DeviceNameFeedback.GetFeedback(Name),
                    StreamUrl,
                    MulticastAddress,
                    VideoName,
                    AudioName,
                    DeviceMode
                });

            RegisterForOnlineFeedback(Hardware, props);
        }

        public StringFeedback AudioName
        {
            get { return _audioName; }
        }

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

        public int DeviceId
        {
            get { return _deviceId; }
        }

        public IntFeedback DeviceMode
        {
            get { return _deviceMode; }
        }

        public new DmNvxBaseClass Hardware
        {
            get { return _hardware; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _inputPorts; }
        }

        public bool IsTransmitter
        {
            get { return _isTransmitter; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _multicastAddress; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _outputPorts; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _secondaryAudioAddress; }
        }

        public StringFeedback StreamUrl
        {
            get { return _streamUrl; }
        }

        public StringFeedback VideoName
        {
            get { return _videoName; }
        }

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
    }
}