using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.InputSwitching
{
    public class NaxInputSwitcher : ICurrentNaxInput
    {
        private readonly StringFeedback _currentNaxInput;
        private readonly IntFeedback _currentNaxInputValue;
        private readonly INvxDeviceWithHardware _device;

        public NaxInputSwitcher(INvxDeviceWithHardware device)
        {
            _device = device;

            _currentNaxInput = NaxInputFeedback.GetFeedback(Hardware);
            _currentNaxInputValue = NaxInputValueFeedback.GetFeedback(Hardware);

            _device.Feedbacks.Add(CurrentNaxInput);
            _device.Feedbacks.Add(CurrentNaxInputValue);
        }

        public StringFeedback CurrentNaxInput
        {
            get { return _currentNaxInput; }
        }

        public IntFeedback CurrentNaxInputValue
        {
            get { return _currentNaxInputValue; }
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