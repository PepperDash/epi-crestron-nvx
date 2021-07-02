using System;
using System.Linq;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Features.Config;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices
{
    public class NvxMockDevice : EssentialsDevice, IStream, ISecondaryAudioStream, IRoutingNumeric
    {
        private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
            new RoutingPortCollection<RoutingInputPort>();

        private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
            new RoutingPortCollection<RoutingOutputPort>();

        public NvxMockDevice(DeviceConfig dc)
            : base(dc.Key, dc.Name)
        {
            var props = dc.Properties.ToObject<NvxMockDeviceProperties>();
            if (props == null)
            {
                Debug.Console(1, this, "************ PROPS IS NULL ************");
                throw new NullReferenceException("props");
            }

            Feedbacks = new FeedbackCollection<Feedback>();

            DeviceId = props.DeviceId;
            IsTransmitter = true;
            BuildFeedbacks(props);
            BuildInputPorts();
        }

        private void BuildFeedbacks(NvxMockDeviceProperties props)
        {
            IsOnline = new BoolFeedback("IsOnline", () => true);
            DeviceMode = new IntFeedback(() => 0);
            StreamUrl = new StringFeedback("StreamUrl",
                () => !String.IsNullOrEmpty(props.StreamUrl) ? props.StreamUrl : String.Empty);

            MulticastAddress = new StringFeedback("MulticastVideoAddress",
                () => !String.IsNullOrEmpty(props.MulticastVideoAddress) ? props.MulticastVideoAddress : String.Empty);

            IsStreamingVideo = new BoolFeedback(() => !String.IsNullOrEmpty(props.StreamUrl));

            VideoStreamStatus = new StringFeedback(
                () => !String.IsNullOrEmpty(props.StreamUrl) ? "Streaming" : String.Empty);

            SecondaryAudioAddress = new StringFeedback(
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress) ? props.MulticastAudioAddress : String.Empty);

            TxAudioAddress = new StringFeedback("MulticastAudio",
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress) ? props.MulticastAudioAddress : String.Empty);

            RxAudioAddress = new StringFeedback(() => string.Empty);

            IsStreamingSecondaryAudio = new BoolFeedback(
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress));

            SecondaryAudioStreamStatus = new StringFeedback(
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress) ? "Streaming" : String.Empty);

            Feedbacks.AddRange(new Feedback[]
                {
                    DeviceNameFeedback.GetFeedback(Name),
                    IsOnline,
                    StreamUrl,
                    MulticastAddress,
                    TxAudioAddress
                });
        }

        private void BuildInputPorts()
        {
            InputPorts.Add(
                new RoutingInputPort(
                    DeviceInputEnum.Hdmi1.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    DeviceInputEnum.Hdmi1,
                    this));

            InputPorts.Add(
                new RoutingInputPort(
                    DeviceInputEnum.SecondaryAudio.Name,
                    eRoutingSignalType.Audio,
                    eRoutingPortConnectionType.Streaming,
                    DeviceInputEnum.SecondaryAudio,
                    this));

            OutputPorts.Add(
                new RoutingOutputPort(
                    SwitcherForStreamOutput.Key,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Streaming,
                    null,
                    this));

            OutputPorts.Add(
                new RoutingOutputPort(
                    SwitcherForSecondaryAudioOutput.Key,
                    eRoutingSignalType.Audio,
                    eRoutingPortConnectionType.LineAudio,
                    null,
                    this));
        }

        public override bool CustomActivate()
        {
            Feedbacks.ToList().ForEach(x => x.FireUpdate());
            return base.CustomActivate();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _inputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _outputPorts; }
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            Debug.Console(2, this, "Executing switch : {0}", signalType);
        }

        public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
        {
            Debug.Console(2, this, "Executing switch : {0}", type);
        }

        public FeedbackCollection<Feedback> Feedbacks { get; private set; }
        public BoolFeedback IsOnline { get; private set; }
        public IntFeedback DeviceMode { get; private set; }
        public bool IsTransmitter { get; private set; }
        public int DeviceId { get; private set; }
        public StringFeedback StreamUrl { get; private set; }
        public StringFeedback SecondaryAudioAddress { get; private set; }
        public StringFeedback TxAudioAddress { get; private set; }
        public StringFeedback RxAudioAddress { get; private set; }
        public BoolFeedback IsStreamingVideo { get; private set; }
        public StringFeedback VideoStreamStatus { get; private set; }
        public BoolFeedback IsStreamingSecondaryAudio { get; private set; }
        public StringFeedback SecondaryAudioStreamStatus { get; private set; }
        public StringFeedback MulticastAddress { get; private set; }
    }
}