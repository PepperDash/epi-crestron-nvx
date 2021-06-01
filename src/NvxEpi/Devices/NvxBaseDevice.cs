using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Features.Config;
using NvxEpi.Features.InputSwitching;
using NvxEpi.Features.Streams.Audio;
using NvxEpi.Features.Streams.Video;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.Utilities;
using NvxEpi.Services.Messages;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Queues;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices
{
    public abstract class NvxBaseDevice : 
        EssentialsBridgeableDevice, 
        ICurrentVideoInput, 
        ICurrentAudioInput, 
        ICurrentStream,
        ICurrentSecondaryAudioStream, 
        ICurrentNaxInput
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

        private const string _showNvxCmd = "shownvxinfo";
        private const string _showNvxCmdHelp = "Prints all keyed feedback status";

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

            AddPreActivationAction(() => Hardware = getHardware());
            AddPreActivationAction(() => IsOnline = new BoolFeedback("IsOnline", () => Hardware.IsOnline));
            AddPreActivationAction(() => RegisterForOnlineFeedback(Hardware, props));
        }

        public override bool CustomActivate()
        {
            DeviceMode = DeviceModeFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[] 
                {
                    IsOnline,
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
            _queue.Enqueue(new BuildNvxDeviceMessage(Key, Hardware));

            return base.CustomActivate();
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
                    Feedbacks.ForEach(f =>
                    {
                        if (f == null)
                            return;

                        f.FireUpdate();
                    });

                    if (!args.DeviceOnLine)
                        return;

                    Hardware.Control.Name.StringValue = Key.Replace(' ', '-');

                    if (IsTransmitter)
                        Hardware.SetTxDefaults(props);
                    else
                        Hardware.SetRxDefaults(props);

                    Hardware.SetAudioDefaults(props);
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

        public BoolFeedback IsOnline { get; private set; }
    }
}