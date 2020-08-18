using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Builders;
using NvxEpi.Device.Enums;
using NvxEpi.Device.JoinMaps;
using NvxEpi.Device.Services.DeviceExtensions;
using NvxEpi.Device.Services.TrilistExtensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Models
{
    public class NvxDevice : CrestronGenericBridgeableBaseDevice, IRouting, IComPorts, ICec, IIROutputPorts
    {
        public enum DeviceFeedbacks
        {
            DeviceName, DeviceStatus, DeviceMode, SecondaryAudioStatus, NaxTxStatus, NaxRxStatus, StreamUrl, HdmiOutputHorizontalResolution,
            Hdmi1SyncDetected, Hdmi2SyncDetected, Hdmi1HdcpCapabilityName, Hdmi2HdcpCapabilityName, HdmiOutputDisabledByHdcp,
            VideoInputName, VideoInputValue, AudioInputName, AudioInputValue, NaxInput, MulticastAddress, SecondaryAudioAddress,
            NaxTxAddress, NaxRxAddress, VideowallMode, CurrentVideoRouteName, CurrentAudioRouteName, CurrentUsbRouteName,
            CurrentUsbRouteValue, UsbMode, Hdmi1HdcpCapabilityValue, Hdmi2HdcpCapabilityValue
        }

        public enum BoolActions
        {
            
        }

        public enum IntActions
        {
            VideoInputSelect, AudioInputSelect, NaxInputSelect, Hdmi1HdcpCapability, Hdmi2HdcpCapability, VideowallMode
        }

        public enum StringActions
        {
            RouteVideo, RouteAudio, StreamUrl, SecondaryAudioAddress, NaxTxAddress, NaxRxAddress, UsbRemoteId
        }

        public new DmNvxBaseClass Hardware { get; private set; }
        public DeviceConfig Config { get; private set; }

        public bool IsTransmitter { get { return _isTransmitter; } }

        private readonly Dictionary<DeviceFeedbacks, Feedback> _feedbacks;
        private readonly Dictionary<BoolActions, Action<bool>> _boolActions;
        private readonly Dictionary<IntActions, Action<ushort>> _intActions;
        private readonly Dictionary<StringActions, Action<string>> _stringActions;

        public bool HasNaxRoutingCapability 
        {
            get { return _feedbacks.ContainsKey(DeviceFeedbacks.NaxInput); }
        }

        private readonly bool _isTransmitter;
        private readonly RoutingPortCollection<RoutingInputPort> _inputs = new RoutingPortCollection<RoutingInputPort>();
        private readonly RoutingPortCollection<RoutingOutputPort> _outputs = new RoutingPortCollection<RoutingOutputPort>(); 

        static NvxDevice()
        {
            CrestronConsole.AddNewConsoleCommand(s => PrintDevicesInfo(), "shownvxdevices",
                "Shows all NVX device informations", ConsoleAccessLevelEnum.AccessAdministrator);
        }

        public string StreamUrl
        {
            get
            {
                Feedback feedback;
                return _feedbacks.TryGetValue(DeviceFeedbacks.StreamUrl, out feedback) ?
                    feedback.StringValue : String.Empty;
            }
        }

        public string MulticastAddress
        {
            get
            {
                Feedback feedback;
                return _feedbacks.TryGetValue(DeviceFeedbacks.MulticastAddress, out feedback) ?
                    feedback.StringValue : String.Empty;
            }
        }

        public string MulticastAudioAddress
        {
            get
            {
                Feedback feedback;
                if (!HasNaxRoutingCapability)
                    return _feedbacks.TryGetValue(DeviceFeedbacks.SecondaryAudioAddress, out feedback)
                        ? feedback.StringValue
                        : String.Empty;

                if (IsTransmitter)
                {
                    return _feedbacks.TryGetValue(DeviceFeedbacks.NaxTxAddress, out feedback)
                        ? feedback.StringValue
                        : String.Empty;
                }

                return _feedbacks.TryGetValue(DeviceFeedbacks.NaxRxAddress, out feedback)
                    ? feedback.StringValue
                    : String.Empty;
            }
        }

        public NvxDevice(INvxDeviceBuilder builder)
            : base(builder.Key, builder.Name, builder.Device)
        {
            Config = builder.Config;
            Hardware = builder.Device;

            _isTransmitter = builder.IsTransmitter;
            _boolActions = builder.BoolActions;
            _intActions = builder.IntActions;
            _stringActions = builder.StringActions;
            _feedbacks = builder.Feedbacks;

            Feedbacks.AddRange(_feedbacks.Values);
        }

        public override bool CustomActivate()
        {
            _feedbacks
                .Values
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
                .OfType<NvxDevice>();

            foreach (var device in devices)
                device.PrintInfoToConsole();
        }

        public void PrintInfoToConsole()
        {
            foreach (var feedback in Feedbacks)
            {
                if (feedback is BoolFeedback)
                    Debug.Console(1, this, "{0} : '{1}'", feedback.Key, feedback.BoolValue);

                if (feedback is IntFeedback)
                    Debug.Console(1, this, "{0} : '{1}'", feedback.Key, feedback.IntValue);

                if (feedback is StringFeedback)
                    Debug.Console(1, this, "{0} : '{1}'", feedback.Key, feedback.StringValue);
            }
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var input = inputSelector as Action;
            if (input == null)
                return;
            
            input.Invoke();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _inputs; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _outputs; }
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public int NumberOfComPorts
        {
            get { return Hardware.NumberOfComPorts; }
        }

        public Cec StreamCec
        {
            get
            {
                if (Hardware.HdmiOut == null)
                    throw new NotSupportedException("hdmi output");

                return Hardware.HdmiOut.StreamCec;
            }
        }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return Hardware.IROutputPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return Hardware.NumberOfIROutputPorts; }
        }

        public void RouteVideo(string key)
        {
            Action<string> routeAction;
            if (!_stringActions.TryGetValue(StringActions.RouteVideo, out routeAction))
                throw new NotSupportedException(StringActions.RouteVideo.ToString());

            routeAction.Invoke(key);
        }

        public void RouteAudio(string key)
        {
            Action<string> routeAction;
            if (!_stringActions.TryGetValue(StringActions.RouteAudio, out routeAction))
                throw new NotSupportedException(StringActions.RouteAudio.ToString());

            routeAction.Invoke(key);
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxDeviceJoinMap(joinStart);

            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);

            trilist
                .BuildFeedbackList(_feedbacks, joinMap)
                .BuildBoolActions(_boolActions, joinMap)
                .BuildIntActions(_intActions, joinMap)
                .BuildStringActions(_stringActions, joinMap);

        }
    }
}