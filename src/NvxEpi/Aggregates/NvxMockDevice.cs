using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Config;
using NvxEpi.Enums;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Aggregates
{
    public class NvxMockDevice : EssentialsDevice, IStream, ISecondaryAudioStream, IRouting, IRoutingNumeric
    {
        private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
            new RoutingPortCollection<RoutingInputPort>();

        private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
            new RoutingPortCollection<RoutingOutputPort>();

        public NvxMockDevice(DeviceConfig dc)
            : base(dc.Key, dc.Name)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(dc);
            DeviceId = props.DeviceId;

            Feedbacks = new FeedbackCollection<Feedback>();
            IsOnline = new BoolFeedback(() => true);
            DeviceMode = new IntFeedback(DeviceModeFeedback.Key, () => (int)eDeviceMode.Transmitter);
            IsTransmitter = true;

            VideoName = String.IsNullOrEmpty(props.VideoSourceName)
                ? new StringFeedback("VideoName", () => Name)
                : new StringFeedback("VideoName", () => props.VideoSourceName);

            AudioName = String.IsNullOrEmpty(props.AudioSourceName)
                ? new StringFeedback("AudioName", () => Name)
                : new StringFeedback("AudioName", () => props.AudioSourceName);

            StreamUrl = new StringFeedback(
                () => !String.IsNullOrEmpty(props.StreamUrl) ? props.StreamUrl : String.Empty);

            IsStreamingVideo = new BoolFeedback(
                () => !String.IsNullOrEmpty(props.StreamUrl));

            VideoStreamStatus = new StringFeedback(
                () => !String.IsNullOrEmpty(props.StreamUrl) ? "Streaming" : String.Empty);

            SecondaryAudioAddress = new StringFeedback(
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress) ? props.MulticastAudioAddress : String.Empty);

            IsStreamingSecondaryAudio = new BoolFeedback(
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress));

            SecondaryAudioStreamStatus = new StringFeedback(
                () => !String.IsNullOrEmpty(props.MulticastAudioAddress) ? "Streaming" : String.Empty);

            Feedbacks.AddRange(new Feedback[]
                {
                    DeviceNameFeedback.GetFeedback(Name),
                    VideoName,
                    AudioName,
                    DeviceMode
                });


            InputPorts.Add(
                new RoutingInputPort(
                    DeviceInputEnum.Hdmi1.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    DeviceInputEnum.Hdmi1,
                    this));

            OutputPorts.Add(
                new RoutingOutputPort(
                StreamOutput.Key,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                null,
                this));

            OutputPorts.Add(
                new RoutingOutputPort(
                SecondaryAudioOutput.Key,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.LineAudio,
                null,
                this));
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
        public StringFeedback VideoName { get; private set; }
        public StringFeedback AudioName { get; private set; }
        public BoolFeedback IsStreamingVideo { get; private set; }
        public StringFeedback VideoStreamStatus { get; private set; }
        public BoolFeedback IsStreamingSecondaryAudio { get; private set; }
        public StringFeedback SecondaryAudioStreamStatus { get; private set; }
    }
}