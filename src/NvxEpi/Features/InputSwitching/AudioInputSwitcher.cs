using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.InputSwitching
{
    public class AudioInputSwitcher : ICurrentAudioInput
    {
        private readonly StringFeedback _currentAudioInput;
        private readonly IntFeedback _currentAudioInputValue;
        private readonly INvxDeviceWithHardware _device;

        public AudioInputSwitcher(INvxDeviceWithHardware device)
        {
            _device = device;

            _currentAudioInput = AudioInputFeedback.GetFeedback(Hardware);
            _currentAudioInputValue = AudioInputValueFeedback.GetFeedback(Hardware);

            _device.Feedbacks.Add(CurrentAudioInput);
            _device.Feedbacks.Add(CurrentAudioInputValue);
        }

        public StringFeedback CurrentAudioInput
        {
            get { return _currentAudioInput; }
        }

        public IntFeedback CurrentAudioInputValue
        {
            get { return _currentAudioInputValue; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
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

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }
    }
}