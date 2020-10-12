using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.InputSwitching
{
    public class AudioInputSwitcher : ICurrentAudioInput
    {
        private readonly INvxDevice _device;

        public AudioInputSwitcher(INvxDevice device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            CurrentAudioInput = AudioInputFeedback.GetFeedback(Hardware);
            CurrentAudioInputValue = AudioInputValueFeedback.GetFeedback(Hardware);

            _device.Feedbacks.Add(CurrentAudioInput);
            _device.Feedbacks.Add(CurrentAudioInputValue);
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public void UpdateDeviceId(uint id)
        {
            _device.UpdateDeviceId(id);
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public StringFeedback CurrentAudioInput { get; private set; }
        public IntFeedback CurrentAudioInputValue { get; private set; }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }
    }
}