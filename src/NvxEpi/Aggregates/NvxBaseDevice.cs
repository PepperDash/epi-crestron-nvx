using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.InputSwitching;
using NvxEpi.Entities.Streams.Audio;
using NvxEpi.Entities.Streams.Video;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.Utilities;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Aggregates
{
    public abstract class NvxBaseDevice : CrestronGenericBridgeableBaseDevice, ICurrentVideoInput, ICurrentAudioInput, ICurrentStream,
        ICurrentSecondaryAudioStream
    {
        private readonly int _deviceId;

        private readonly DmNvxBaseClass _hardware;
        private readonly ICurrentSecondaryAudioStream _currentSecondaryAudioStream;
        private readonly ICurrentStream _currentVideoStream;
        private readonly ICurrentVideoInput _videoSwitcher;
        private readonly ICurrentAudioInput _audioSwitcher;

        private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
            new RoutingPortCollection<RoutingInputPort>();

        private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
            new RoutingPortCollection<RoutingOutputPort>();

        private readonly IntFeedback _deviceMode;

        private readonly StringFeedback _audioSourceName;
        private readonly StringFeedback _audioDestinationName;
        private readonly StringFeedback _videoName;

        private const string _showNvxCmd = "shownvxinfo";
        private const string _showNvxCmdHelp = "Prints all keyed feedback status";

        static NvxBaseDevice()
        {
            CrestronConsole.AddNewConsoleCommand(s => DeviceConsole.PrintInfoForAllDevices(),
                _showNvxCmd,
                _showNvxCmdHelp,
                ConsoleAccessLevelEnum.AccessAdministrator);
        }

        protected NvxBaseDevice(DeviceConfig config, DmNvxBaseClass hardware)
            : base(config.Key, config.Name, hardware)
        {
            _hardware = hardware;

            var props = NvxDeviceProperties.FromDeviceConfig(config);
            _deviceId = props.DeviceId;

            if (hardware is DmNvx35x || hardware is DmNvx36x)
            {
                IsTransmitter = !String.IsNullOrEmpty(props.Mode) &&
                                props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);
            }
            else if (hardware is DmNvxD3x)
            {
                IsTransmitter = false;
            }
            else if (hardware is DmNvxE3x || hardware is DmNvxE760x)
            {
                IsTransmitter = true;
            }
            else
            {
                throw new Exception(string.Format("Type is not yet accounted for : {0}", hardware.GetType().Name));
            }

            _currentVideoStream = new CurrentVideoStream(this);
            _currentSecondaryAudioStream = new CurrentSecondaryAudioStream(this);
            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);

            _videoName = String.IsNullOrEmpty(props.VideoName)
                ? new StringFeedback("VideoName", () => Name)
                : new StringFeedback("VideoName", () => props.VideoName);

            _audioSourceName = String.IsNullOrEmpty(props.AudioSourceName)
                ? new StringFeedback("AudioSourceName", () => Name)
                : new StringFeedback("AudioSourceName", () => props.AudioSourceName);

            _audioDestinationName = String.IsNullOrEmpty(props.AudioDestinationName)
                ? new StringFeedback("AudioDestinationName", () => Name)
                : new StringFeedback("AudioDestinationName", () => props.AudioDestinationName);

            _deviceMode = DeviceModeFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
                {
                    DeviceNameFeedback.GetFeedback(Name),
                    DeviceIpFeedback.GetFeedback(Hardware),
                    DeviceHostnameFeedback.GetFeedback(Hardware),
                    DeviceModeNameFeedback.GetFeedback(Hardware),
                    VideoName,
                    AudioSourceName,
                    AudioDestinationName,
                    DeviceMode
                });

            RegisterForOnlineFeedback(Hardware, props);
        }

        public StringFeedback AudioSourceName
        {
            get { return _audioSourceName; }
        }

        public StringFeedback AudioDestinationName
        {
            get { return _audioDestinationName; }
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

        public bool IsTransmitter { get; private set; }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _outputPorts; }
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
                    Feedbacks.ForEach(f => f.FireUpdate());
                    if (!args.DeviceOnLine)
                        return;

                    Hardware.Control.Name.StringValue = Key.Replace(' ', '-');
                };
        }

        public StringFeedback StreamUrl
        {
            get { return _currentVideoStream.StreamUrl; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _currentVideoStream.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _currentVideoStream.VideoStreamStatus; }
        }

        public StringFeedback CurrentStreamName
        {
            get { return _currentVideoStream.CurrentStreamName; }
        }

        public IntFeedback CurrentStreamId
        {
            get { return _currentVideoStream.CurrentStreamId; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioAddress; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _currentSecondaryAudioStream.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioStreamStatus; }
        }

        public StringFeedback CurrentSecondaryAudioStreamName
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamName; }
        }

        public IntFeedback CurrentSecondaryAudioStreamId
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamId; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _currentVideoStream.MulticastAddress; }
        }
    }
}