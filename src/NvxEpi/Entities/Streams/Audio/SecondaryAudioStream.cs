using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Audio
{
    public class SecondaryAudioStream : ISecondardyAudioStreamWithHardware
    {
        private readonly INvx35xDeviceWithHardware _device;

        private readonly StringFeedback _secondaryAudioAddress;
        private readonly BoolFeedback _isStreamingSecondaryAudio;
        private readonly StringFeedback _secondaryAudioStreamStatus;

        public SecondaryAudioStream(INvx35xDeviceWithHardware device)
        {
            _device = device;

            _secondaryAudioStreamStatus = SecondaryAudioStatusFeedback.GetFeedback(Hardware);
            _isStreamingSecondaryAudio = IsStreamingSecondaryAudioFeedback.GetFeedback(Hardware);
            _secondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);

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

        DmNvxBaseClass INvxHardware.Hardware
        {
            get { return _device.Hardware; }
        }

        public DmNvx35x Hardware
        {
            get { return _device.Hardware; }
        }
    }
}