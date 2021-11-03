using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Audio
{
    public class SecondaryAudioStream : ISecondaryAudioStreamWithHardware
    {
        private readonly INvxDeviceWithHardware _device;

        private readonly StringFeedback _secondaryAudioRxAddress;
        private readonly StringFeedback _secondaryAudioTxAddress;
        private readonly BoolFeedback _isStreamingSecondaryAudio;
        private readonly StringFeedback _secondaryAudioStreamStatus;
        private readonly StringFeedback _secondaryAudioAddres;

        public SecondaryAudioStream(INvxDeviceWithHardware device)
        {
            _device = device;

            _secondaryAudioStreamStatus = SecondaryAudioStatusFeedback.GetFeedbackForReceiver(Hardware);
            _isStreamingSecondaryAudio = IsStreamingSecondaryAudioFeedback.GetFeedbackForReceiver(Hardware);
            _secondaryAudioRxAddress = AudioRxAddressFeedback.GetFeedback(Hardware);
            _secondaryAudioTxAddress = AudioTxAddressFeedback.GetFeedback(Hardware);
            _secondaryAudioAddres = IsTransmitter
                ? SecondaryAudioAddressFeedback.GetFeedbackForTransmitter(Hardware)
                : SecondaryAudioAddressFeedback.GetFeedbackForReceiver(Hardware);

            Feedbacks.Add(SecondaryAudioAddress);
            Feedbacks.Add(SecondaryAudioStreamStatus);
            Feedbacks.Add(IsStreamingSecondaryAudio);
            Feedbacks.Add(TxAudioAddress);
            Feedbacks.Add(RxAudioAddress);
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
            get 
            {
                return _secondaryAudioAddres;
            }
        }

        public StringFeedback TxAudioAddress
        {
            get { return _secondaryAudioTxAddress; }
        }

        public StringFeedback RxAudioAddress
        {
            get { return _secondaryAudioRxAddress; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }
    }
}