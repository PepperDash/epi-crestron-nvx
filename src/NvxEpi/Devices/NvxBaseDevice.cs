using System;
using System.Linq;
using System.Text.RegularExpressions;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Features.Config;
using NvxEpi.Features.InputSwitching;
using NvxEpi.Features.Monitor;
using NvxEpi.Features.Streams.Audio;
using NvxEpi.Features.Streams.Video;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.Utilities;
using NvxEpi.Services.Messages;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.DeviceInfo;
using PepperDash.Essentials.Core.Queues;
using PepperDash.Essentials.Core.Config;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using PepperDash.Core;
using NvxEpi.Abstractions.HdmiOutput;


#if SERIES4
using NvxEpi.McMessengers;
#endif


namespace NvxEpi.Devices
{
    public abstract class NvxBaseDevice : 
        EssentialsBridgeableDevice, 
        ICurrentVideoInput, 
        ICurrentAudioInput, 
        ICurrentStream,
        ICurrentSecondaryAudioStream, 
        ICurrentNaxInput,
        ICommunicationMonitor,
        IDeviceInfoProvider
    {
        private ICurrentSecondaryAudioStream _currentSecondaryAudioStream;
        private ICurrentStream _currentVideoStream;
        private ICurrentVideoInput _videoSwitcher;
        private ICurrentAudioInput _audioSwitcher;
        private ICurrentNaxInput _naxSwitcher;

        private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
            new RoutingPortCollection<RoutingInputPort>();

        private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
            new RoutingPortCollection<RoutingOutputPort>();

        private string _hardwareName;
        private const string _showNvxCmd = "shownvxinfo";
        private const string _showNvxCmdHelp = "Prints all keyed feedback status";

        public readonly string DefaultMulticastRoute;

        private static IQueue<IQueueMessage> _queue;

        static NvxBaseDevice()
        {
            CrestronConsole.AddNewConsoleCommand(s => DeviceConsole.PrintInfoForAllDevices(),
                _showNvxCmd,
                _showNvxCmdHelp,
                ConsoleAccessLevelEnum.AccessAdministrator);
        }

        protected NvxBaseDevice(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter)
            : base(config.Key, config.Name)
        {
            if (getHardware == null)
                throw new ArgumentNullException("hardware");

            if (_queue == null)
                _queue = new GenericQueue("NvxDeviceBuildQueue", Thread.eThreadPriority.LowestPriority, 200);

            Feedbacks = new FeedbackCollection<Feedback>();
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            DeviceId = props.DeviceId;
            IsTransmitter = isTransmitter;

            AutomaticInput.AddRoutingPort(this);
            NoSwitchInput.AddRoutingPort(this);

            DefaultMulticastRoute = props.DefaultMulticastSource;

            SetDeviceName();

            AddPreActivationAction(() => Hardware = getHardware());
            AddPreActivationAction(() => CommunicationMonitor = new NvxCommunicationMonitor(this, 10000, 30000, Hardware));
            AddPreActivationAction(() => RegisterForOnlineFeedback(Hardware, props));
        }

        private void SetDeviceName()
        {
            var tempName = string.IsNullOrEmpty(Name) ? Key : Name;
            tempName = tempName.Replace(' ', '-');
            var r = new Regex("[^a-zA-Z0-9-]"); //Replace all except alphanumeric and dash
            _hardwareName = r.Replace(tempName, "");
        }

        public override bool CustomActivate()
        {
            Debug.Console(1, this, "Activating...");
            DeviceMode = DeviceModeFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[] 
                {
                    IsOnline,
                    new IntFeedback("DeviceId", () => DeviceId), 
                    DeviceNameFeedback.GetFeedback(Name),
                    DeviceIpFeedback.GetFeedback(Hardware),
                    DeviceHostnameFeedback.GetFeedback(Hardware),
                    DeviceModeNameFeedback.GetFeedback(Hardware),
                    DanteInputFeedback.GetFeedback(Hardware),
                    DanteInputValueFeedback.GetFeedback(Hardware),
                    DeviceMode
                });

            _currentVideoStream = new CurrentVideoStream(this);
            _currentSecondaryAudioStream = new CurrentSecondaryAudioStream(this);
            _videoSwitcher = new VideoInputSwitcher(this);
            _audioSwitcher = new AudioInputSwitcher(this);
            _naxSwitcher = new NaxInputSwitcher(this);

            RegisterForFeedback();
            CommunicationMonitor.Start();
            Hardware.Network.NetworkChange += (sender, args) => UpdateDeviceInfo();

            _queue.Enqueue(new BuildNvxDeviceMessage(Key, Hardware));

            if (IsTransmitter || Hardware == null) return base.CustomActivate();
            if (Hardware.Control.ServerUrlFeedback.StringValue != String.Empty)
                Hardware.Control.ServerUrl.StringValue = String.Empty;
            Hardware.Control.ServerUrl.StringValue = DefaultMulticastRoute;

            return base.CustomActivate();
        }

        protected void AddMcMessengers()
        {
#if SERIES4
            var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();

            if (mc == null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Mobile Control not found", this);
                return;
            }

            var streamMessenger = new PrimaryStreamStatusMessenger($"{Key}-streamStatus", $"/device/{Key}", this);

            mc.AddDeviceMessenger(streamMessenger);

            var secondaryAudioMessenger = new SecondaryAudioStatusMessenger($"{Key}-secondaryAudioStreamStatus", $"/device/{Key}", this);

            mc.AddDeviceMessenger(secondaryAudioMessenger);

            var infoMessenger = new EndpointInfoMessenger($"{Key}-endpointInfo", $"/device/{Key}", this);

            mc.AddDeviceMessenger(infoMessenger);

            if (!(this is IHdmiInput hdmiInputDevice))
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "{key:l} does NOT implement IHdmiInput interface", this, Key);
                return;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Generating HDMI Input messenger for {key:l}", this, Key);

            
            var hdmiInputMessenger = new IHdmiInputMessenger($"{Key}-hdmiInputMessenger", $"/device/{Key}", hdmiInputDevice);

            mc.AddDeviceMessenger(hdmiInputMessenger);

            if (!(this is IHdmiOutput hdmiOutputDevice))
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information,"{key:l} does NOT implement IHdmiOutput interface", this, Key);
                return;
            }

            var hdmiOutputMessenger = new IHdmiOutputMessenger($"{Key}-hdmiOutputMessenger", $"/device/{Key}", hdmiOutputDevice);

            mc.AddDeviceMessenger(hdmiOutputMessenger);
#endif
        }

        public StringFeedback CurrentAudioInput
        {
            get { return _audioSwitcher.CurrentAudioInput; }
        }

        public StringFeedback CurrentNaxInput
        {
            get { return _naxSwitcher.CurrentNaxInput; }
        }

        public IntFeedback CurrentAudioInputValue
        {
            get { return _audioSwitcher.CurrentAudioInputValue; }
        }

        public IntFeedback CurrentNaxInputValue
        {
            get { return _naxSwitcher.CurrentNaxInputValue; }
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

        public DmNvxBaseClass Hardware { get; private set; }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _inputPorts; }
        }

        public bool IsTransmitter { get; private set; }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _outputPorts; }
        }

        public override string ToString()
        {
            return Key;
        }

        private void RegisterForOnlineFeedback(GenericBase hardware, NvxDeviceProperties props)
        {
            hardware.OnlineStatusChange += (device, args) =>
                {
                    Feedbacks
                        .Where(x => x != null)
                        .ToList()
                        .ForEach(f => f.FireUpdate());

                    if (!args.DeviceOnLine)
                        return;

                    Hardware.Control.Name.StringValue = _hardwareName;

                    if (IsTransmitter || hardware is DmNvxE30)
                        Hardware.SetTxDefaults(props);
                    else
                        Hardware.SetRxDefaults(props);
                };
        }

        private void RegisterForFeedback()
        {
            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
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

        public StringFeedback TxAudioAddress
        {
            get { return _currentSecondaryAudioStream.TxAudioAddress; }
        }

        public StringFeedback RxAudioAddress
        {
            get { return _currentSecondaryAudioStream.RxAudioAddress; }
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

        public FeedbackCollection<Feedback> Feedbacks { get; private set; }

        public BoolFeedback IsOnline
        {
            get { return CommunicationMonitor.IsOnlineFeedback; }
        }

        public StatusMonitorBase CommunicationMonitor { get; private set; }

        public void UpdateDeviceInfo()
        {
            DeviceInfo = new DeviceInfo
            {
                IpAddress = Hardware.Network.IpAddressFeedback.StringValue,
                HostName = Hardware.Network.HostNameFeedback.StringValue,
                MacAddress = string.Empty,
                FirmwareVersion = string.Empty
            };

            var handler = DeviceInfoChanged;
            if (handler == null)
                return;

            handler(this, new DeviceInfoEventArgs { DeviceInfo = DeviceInfo });
        }

        public DeviceInfo DeviceInfo { get; private set; }
        public event DeviceInfoChangeHandler DeviceInfoChanged;
    }
}