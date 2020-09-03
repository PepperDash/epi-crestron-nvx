using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Builders;
using NvxEpi.Device.JoinMaps;
using NvxEpi.Device.Models.Entities;
using NvxEpi.Device.Services.DeviceExtensions;
using NvxEpi.Device.Services.TrilistExtensions;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Aggregates
{
    public class Nvx35x : CrestronGenericBridgeableBaseDevice, IComPorts, ICec, IIROutputPorts, IVideoInputSwitcher,
        IAudioInputSwitcher, IHdmiInputs, IHdmiOutput, IVideoStreamRouting, IAudioStreamRouting, IRouting
    {
        private readonly INvxDevice _device;
        private readonly IVideoInputSwitcher _videoInput;
        private readonly IAudioInputSwitcher _audioInput;
        private readonly IAudioStream _audioStream;
        private readonly IHdmiInputs _hdmiInputs;
        private readonly IHdmiOutput _hdmiOutput;
        private readonly IVideoStreamRouting _videoStreamRouting;
        private readonly IAudioStreamRouting _audioStreamRouting;
        private readonly IRouting _router;

        public Nvx35x(INvxDevice device)
            : base(device.Key, device.Name, device.Hardware)
        {
            _device = device;         
            _videoInput = new VideoInputSwitcher(_device);
            _audioInput = new AudioInputSwitcher(_device);
            _videoStreamRouting = new PrimaryVideoRouter(_device);
            _audioStream = new AudioStream(_device);
            _audioStreamRouting = new SecondaryAudioRouter(_audioStream);
            _hdmiInputs = new NvxHdmiInputs(_device);
            _hdmiOutput = new NvxHdmiOutput(_device);
            _router = new Nvx35xRouter(_videoInput, _audioInput);

            Feedbacks.AddRange(_device.Feedbacks);
        }

        public override bool CustomActivate()
        {
            _device.RegisterForDeviceFeedback();

            _device.UsageTracker = new UsageTracking(this);
            UsageTracker = _device.UsageTracker;

            Hardware.OnlineStatusChange += (device, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                _device.SetDeviceDefaults();
            };

            Feedbacks
                .ToList()
                .ForEach(feedback => feedback.OutputChange += FeedbackOnOutputChange);

            return base.CustomActivate();
        }

        private void FeedbackOnOutputChange(object sender, FeedbackEventArgs feedbackEventArgs)
        {
            var keyed = sender as IKeyed;
            if (keyed == null)
                return;

            if (sender is BoolFeedback)
                Debug.Console(1, this, "Received {0} Update : '{1}'", keyed.Key, feedbackEventArgs.BoolValue);

            if (sender is IntFeedback)
                Debug.Console(1, this, "Received {0} Update : '{1}'", keyed.Key, feedbackEventArgs.IntValue);

            if (sender is StringFeedback)
                Debug.Console(1, this, "Received {0} Update : '{1}'", keyed.Key, feedbackEventArgs.StringValue);
        }

        public static void PrintDevicesInfo()
        {
            var devices = DeviceManager
                .GetDevices()
                .OfType<INvxDevice>();

            foreach (var device in devices)
                device.PrintInfoToConsole();
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxDeviceJoinMap(joinStart);

            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);
            CurrentVideoRouteValue.LinkInputSig(trilist.UShortInput[joinMap.VideoRoute.JoinNumber]);
            CurrentVideoRouteName.LinkInputSig(trilist.StringInput[joinMap.VideoRoute.JoinNumber]);
            CurrentAudioRouteValue.LinkInputSig(trilist.UShortInput[joinMap.AudioRoute.JoinNumber]);
            CurrentAudioRouteName.LinkInputSig(trilist.StringInput[joinMap.AudioRoute.JoinNumber]);

            trilist.BuildFeedbackList(Feedbacks, joinMap);
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _router.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _router.OutputPorts; }
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            _router.ExecuteSwitch(inputSelector, outputSelector, signalType);
        }

        public StringFeedback VideoInputName
        {
            get { return _videoInput.VideoInputName; }
        }

        public IntFeedback VideoInputValue
        {
            get { return _videoInput.VideoInputValue; }
        }

        public StringFeedback AudioInputName
        {
            get { return _audioInput.AudioInputName; }
        }

        public IntFeedback AudioInputValue
        {
            get { return _audioInput.AudioInputValue; }
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public int NumberOfComPorts
        {
            get { return Hardware.NumberOfComPorts; }
        }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return Hardware.IROutputPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return Hardware.NumberOfIROutputPorts; }
        }

        public int VirtualDeviceId
        {
            get { return _device.VirtualDeviceId; }
        }

        public DeviceConfig Config
        {
            get { return _device.Config; }
        }

        public BoolFeedback IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _device.IsStreamingVideo; }
        }

        public StringFeedback DeviceName
        {
            get { return _device.DeviceName; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _device.VideoStreamStatus; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public Cec StreamCec 
        {
            get { return Hardware.HdmiOut.StreamCec; }
        }

        public BoolFeedback IsStreamingAudio
        {
            get { return _audioStream.IsStreamingAudio; }
        }

        public StringFeedback AudioStreamStatus
        {
            get { return _audioStream.AudioStreamStatus; }
        }

        public StringFeedback AudioMulticastAddress
        {
            get { return _audioStream.AudioMulticastAddress; }
        }

        public IntFeedback HorizontalResolution
        {
            get { return _hdmiOutput.HorizontalResolution; }
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _hdmiOutput.DisabledByHdcp; }
        }

        public IntFeedback CurrentVideoRouteValue
        {
            get { return _videoStreamRouting.CurrentVideoRouteValue; }
        }

        public StringFeedback CurrentVideoRouteName
        {
            get { return _videoStreamRouting.CurrentVideoRouteName; }
        }

        public IntFeedback CurrentAudioRouteValue
        {
            get { return _audioStreamRouting.CurrentAudioRouteValue; }
        }

        public StringFeedback CurrentAudioRouteName
        {
            get { return _audioStreamRouting.CurrentAudioRouteName; }
        }

        public void SetAudioAddress(string address)
        {
            _audioStreamRouting.SetAudioAddress(address);
        }

        public void StartAudioStream()
        {
            _audioStreamRouting.StartAudioStream();
        }

        public void StopAudioStream()
        {
            _audioStreamRouting.StopAudioStream();
        }

        public ReadOnlyDictionary<uint, IHdmiInput> HdmiInputs
        {
            get { return _hdmiInputs.HdmiInputs; }
        }

        public new DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }
    }
}