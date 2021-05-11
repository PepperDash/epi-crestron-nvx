using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.InputSwitching
{
    public class DanteInputSwitcher : ICurrentDanteInput
    {
        private readonly INvxDeviceWithHardware _device;

        public DanteInputSwitcher(INvxDeviceWithHardware device)
        {
            _device = device;

            CurrentDanteInput = AudioInputFeedback.GetFeedback(Hardware);
            CurrentDanteInputValue = AudioInputValueFeedback.GetFeedback(Hardware);

            _device.Feedbacks.Add(CurrentDanteInput);
            _device.Feedbacks.Add(CurrentDanteInputValue);
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

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
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

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public StringFeedback CurrentDanteInput { get; private set; }
        public IntFeedback CurrentDanteInputValue { get; private set; }
    }
}