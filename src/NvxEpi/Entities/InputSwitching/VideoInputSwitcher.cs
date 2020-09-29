using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.InputSwitching
{
    public class VideoInputSwitcher : ICurrentVideoInput
    {
        private readonly INvxDevice _device;

        public VideoInputSwitcher(INvxDevice device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            CurrentVideoInput = VideoInputFeedback.GetFeedback(Hardware);
            CurrentVideoInputValue = VideoInputValueFeedback.GetFeedback(Hardware);

            _device.Feedbacks.Add(CurrentVideoInput);
            _device.Feedbacks.Add(CurrentVideoInputValue);
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

        public StringFeedback CurrentVideoInput { get; private set; }
        public IntFeedback CurrentVideoInputValue { get; private set; }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }
    }
}