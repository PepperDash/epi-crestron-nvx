using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Builders;
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
            DeviceName,
            DeviceStatus,
            DeviceMode,
            SecondaryAudioStatus,
            NaxTxStatus,
            NaxRxStatus,
            StreamUrl,
            HdmiOutputHorizontalResolution,
            Hdmi1SyncDetected,
            Hdmi2SyncDetected,
            Hdmi1HdcpCapabilityName,
            Hdmi2HdcpCapabilityName,
            HdmiOutputDisabledByHdcp,
            VideoInputName,
            VideoInputValue,
            AudioInputName,
            AudioInputValue,
            NaxInput,
            MulticastAddress,
            SecondaryAudioAddress,
            NaxTxAddress,
            NaxRxAddress,
            VideowallMode,
            CurrentVideoRouteName,
            CurrentAudioRouteName,
            CurrentUsbRouteName,
            CurrentUsbRouteValue,
            UsbMode,
            Hdmi1HdcpCapabilityValue,
            Hdmi2HdcpCapabilityValue
        }

        public enum BoolActions
        {
            EnableAudioStream,
            EnableVideoStream
        }

        public enum IntActions
        {
            VideoInputSelect,
            AudioInputSelect,
            NaxInputSelect,
            Hdmi1HdcpCapability,
            Hdmi2HdcpCapability,
            VideowallMode
        }

        public enum StringActions
        {
            StreamUrl,
            SecondaryAudioAddress,
            NaxTxAddress,
            NaxRxAddress,
            UsbRemoteId
        }

        private readonly DmNvxBaseClass _device;
        private readonly Dictionary<DeviceFeedbacks, Feedback> _feedbacks;
        private readonly Dictionary<BoolActions, Action<bool>> _boolActions;
        private readonly Dictionary<IntActions, Action<ushort>> _intActions;
        private readonly Dictionary<StringActions, Action<string>> _stringActions;
        private readonly bool _isTransmitter;
        private readonly RoutingPortCollection<RoutingInputPort> _inputs = 
            new RoutingPortCollection<RoutingInputPort>();

        private readonly RoutingPortCollection<RoutingOutputPort> _outputs =
            new RoutingPortCollection<RoutingOutputPort>();

        public DeviceConfig Config { get; private set; }

        public bool IsTransmitter
        {
            get { return _isTransmitter; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _inputs; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _outputs; }
        }

        public bool HasNaxRoutingCapability
        {
            get { return _feedbacks.ContainsKey(DeviceFeedbacks.NaxInput); }
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return _device.ComPorts; }
        }

        public int NumberOfComPorts
        {
            get { return _device.NumberOfComPorts; }
        }

        public Cec StreamCec
        {
            get
            {
                if (_device.HdmiOut == null)
                    throw new NotSupportedException("hdmi output");

                return _device.HdmiOut.StreamCec;
            }
        }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return _device.IROutputPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return _device.NumberOfIROutputPorts; }
        }

        static NvxDevice()
        {
            CrestronConsole.AddNewConsoleCommand(s => PrintDevicesInfo(), "shownvxdevices",
                "Shows all NVX device informations", ConsoleAccessLevelEnum.AccessAdministrator);
        }

        public bool IsVideoStreaming
        {
            get { return _device.Control.StartFeedback.BoolValue; }
        }

        public bool IsAudioStreaming
        {
            get { return _device.SecondaryAudio != null && _device.SecondaryAudio.StartFeedback.BoolValue; }
        }

        public string StreamUrl
        {
            get
            {
                Feedback feedback;
                if (_feedbacks.TryGetValue(DeviceFeedbacks.StreamUrl, out feedback))
                    feedback.FireUpdate();

                return feedback == null ? String.Empty : feedback.StringValue;
            }
        }

        public string MulticastAddress
        {
            get
            {
                Feedback feedback;
                if (_feedbacks.TryGetValue(DeviceFeedbacks.MulticastAddress, out feedback))
                    feedback.FireUpdate();

                return feedback == null ? String.Empty : feedback.StringValue;
            }
        }

        public string MulticastAudioAddress
        {
            get
            {
                Feedback feedback;
                if(_feedbacks.TryGetValue(DeviceFeedbacks.SecondaryAudioAddress, out feedback))
                    feedback.FireUpdate();

                return feedback == null ? String.Empty : feedback.StringValue;
            }
        }

        public NvxDevice(INvxDeviceBuilder builder)
            : base(builder.Key, builder.Name, builder.Device)
        {
            Config = builder.Config;
            _device = builder.Device;

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
            var input = inputSelector as Action<eRoutingSignalType>;
            if (input == null)
                return;

            input.Invoke(signalType);
        }

        public void SetVideoInput(ushort input)
        {
            Action<ushort> action;
            if (!_intActions.TryGetValue(IntActions.VideoInputSelect, out action))
                return;

            action.Invoke(input);
        }

        public void SetAudioInput(ushort input)
        {
            Action<ushort> action;
            if (!_intActions.TryGetValue(IntActions.AudioInputSelect, out action))
                return;

            action.Invoke(input);
        }

        public void StartVideoStream()
        {
            Action<bool> action;
            if (!_boolActions.TryGetValue(BoolActions.EnableVideoStream, out action))
                return;

            action.Invoke(true);
        }

        public void StopVideoStream()
        {
            Action<bool> action;
            if (!_boolActions.TryGetValue(BoolActions.EnableVideoStream, out action))
                return;

            action.Invoke(false);
        }

        public void StartAudioStream()
        {
            Action<bool> action;
            if (!_boolActions.TryGetValue(BoolActions.EnableAudioStream, out action))
                return;

            action.Invoke(true);
        }

        public void StopAudioStream()
        {
            Action<bool> action;
            if (!_boolActions.TryGetValue(BoolActions.EnableAudioStream, out action))
                return;

            action.Invoke(false);
        }

        public void SetStreamUrl(string streamUrl)
        {
            Action<string> action;
            if (!_stringActions.TryGetValue(StringActions.StreamUrl, out action))
                return;

            action.Invoke(streamUrl);
        }

        public void SetAudioMulticastAddress(string addres)
        {
            Action<string> action;
            if (!_stringActions.TryGetValue(StringActions.SecondaryAudioAddress, out action))
                return;

            action.Invoke(addres);
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxDeviceJoinMap(joinStart);

            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);
            trilist.SetStringSigAction(joinMap.VideoRoute.JoinNumber, s => NvxRouter.RouteVideo(this, s));
            trilist.SetStringSigAction(joinMap.AudioRoute.JoinNumber, s => NvxRouter.RouteAudio(this, s));

            trilist
                .BuildFeedbackList(_feedbacks, joinMap)
                .BuildBoolActions(_boolActions, joinMap)
                .BuildIntActions(_intActions, joinMap)
                .BuildStringActions(_stringActions, joinMap);
        }
    }
}