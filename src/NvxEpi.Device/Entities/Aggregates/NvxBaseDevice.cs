using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Config;
using NvxEpi.Device.Entities.Hardware;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.Services.Bridge;
using NvxEpi.Device.Services.DeviceFeedback;
using NvxEpi.Device.Services.Feedback;
using NvxEpi.Device.Services.InputPorts;
using NvxEpi.Device.Services.InputSwitching;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Entities.Aggregates
{
    public abstract class NvxBaseDevice : CrestronGenericBridgeableBaseDevice, IStream, ICurrentVideoInput, ICurrentAudioInput
    {
        private readonly RoutingPortCollection<RoutingInputPort> _inputs = new RoutingPortCollection<RoutingInputPort>();
        private readonly RoutingPortCollection<RoutingOutputPort> _outputs = new RoutingPortCollection<RoutingOutputPort>();

        protected readonly IStream _stream;
        protected readonly ICurrentVideoInput _videoSwitcher;
        protected readonly ICurrentAudioInput _audioSwitcher;

        protected NvxBaseDevice(DeviceConfig config, DmNvxBaseClass hardware)
            : base(config.Key, config.Name, hardware)
        {
            Key = config.Key;
            Name = config.Name;
            Hardware = hardware;

            var props = NvxDeviceProperties.FromDeviceConfig(config);
            DeviceId = props.DeviceId;
            IsTransmitter = props.IsTransmitter;

            _stream = new VideoStream(this);
            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);

            hardware.OnlineStatusChange += (device, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                Hardware.Control.Name.StringValue = Name.Replace(' ', '-');

                if (IsTransmitter)
                    Hardware.SetTxDefaults(props);
                else
                    Hardware.SetRxDefaults(props);

                Hardware.SetDefaultInputs(props);
            };

            SetupFeedbacks();
            AddRoutingPorts();
        }

        private void SetupFeedbacks()
        {
            DeviceMode = DeviceModeFeedback.GetFeedback(Hardware);
            MulticastAddress = MulticastAddressFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
            {
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

        private void AddRoutingPorts()
        {
            if (IsTransmitter)
            {
                HdmiInput1.AddRoutingPort(_videoSwitcher);
                StreamOutput.AddRoutingPort(_videoSwitcher);
            }
            else
            {
                StreamInput.AddRoutingPort(this);
                PrimaryAudioInput.AddRoutingPort(this);
                HdmiOutput.AddRoutingPort(this);
                AnalogAudioOutput.AddRoutingPort(_audioSwitcher);
            }
        }

        public override bool CustomActivate()
        {
            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
            DeviceDebug.RegisterForRoutingInputPortFeedback(this);
            DeviceDebug.RegisterForRoutingOutputFeedback(this);

            return base.CustomActivate();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get { return _inputs; } }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get { return _outputs; } }

        public bool IsTransmitter { get; private set; }
        public int DeviceId { get; private set; }
        public IntFeedback DeviceMode { get; private set; }
        public StringFeedback MulticastAddress { get; private set; }

        public new DmNvxBaseClass Hardware { get; private set; }

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

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var deviceBridge = new NvxDeviceBridge(this);
            deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);

        }
    }
}