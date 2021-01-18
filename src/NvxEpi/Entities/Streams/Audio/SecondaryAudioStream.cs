using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Audio
{
    public class SecondaryAudioStream : ISecondardyAudioStreamWithHardware
    {
        private readonly INvxDeviceWithHardware _device;

        private readonly StringFeedback _secondaryAudioAddress;
        private readonly BoolFeedback _isStreamingSecondaryAudio;
        private readonly StringFeedback _secondaryAudioStreamStatus;

        public SecondaryAudioStream(INvxDeviceWithHardware device)
        {
            _device = device;

            if (device.IsTransmitter)
            {
                _secondaryAudioStreamStatus = SecondaryAudioStatusFeedback.GetFeedbackForTransmitter(Hardware);
                _isStreamingSecondaryAudio = IsStreamingSecondaryAudioFeedback.GetFeedbackForTransmitter(Hardware);
                _secondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedbackForTransmitter(Hardware);
            }
            else
            {
                _secondaryAudioStreamStatus = SecondaryAudioStatusFeedback.GetFeedbackForReceiver(Hardware);
                _isStreamingSecondaryAudio = IsStreamingSecondaryAudioFeedback.GetFeedbackForReceiver(Hardware);
                _secondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedbackForReceiver(Hardware);
            }

            Feedbacks.Add(SecondaryAudioStreamStatus);
            Feedbacks.Add(IsStreamingSecondaryAudio);
            Feedbacks.Add(SecondaryAudioAddress);
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _isStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _secondaryAudioStreamStatus; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _secondaryAudioAddress; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }
    }
}