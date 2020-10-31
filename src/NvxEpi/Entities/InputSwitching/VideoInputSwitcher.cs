using System;
using System.Globalization;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.InputSwitching
{
    public class VideoInputSwitcher : ICurrentVideoInput
    {
        private readonly INvxHardware _device;

        public VideoInputSwitcher(INvxHardware device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            CurrentVideoInput = VideoInputFeedback.GetFeedback(Hardware);
            CurrentVideoInputValue = VideoInputValueFeedback.GetFeedback(Hardware);

            Feedbacks.Add(CurrentVideoInput);
            Feedbacks.Add(CurrentVideoInputValue);
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

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public void SetVideoInput(ushort input)
        {
            var inputToSwitch = (eSfpVideoSourceTypes)input;

            switch (inputToSwitch)
            {
                case eSfpVideoSourceTypes.Disable:
                    SetVideoToNone();
                    break;
                case eSfpVideoSourceTypes.Hdmi1:
                    SetVideoToHdmiInput1();
                    break;
                case eSfpVideoSourceTypes.Hdmi2:
                    SetVideoToHdmiInput2();
                    break;
                case eSfpVideoSourceTypes.Stream:
                    SetVideoToStream();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(input.ToString(CultureInfo.InvariantCulture));
            }
        }

        public void SetVideoToHdmiInput1()
        {
            if (Hardware is DmNvxD3x)
                return;

            Debug.Console(1, this, "Switching Video Input to : 'Hdmi1'");
            Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
        }

        public void SetVideoToHdmiInput2()
        {
            if (!(Hardware is DmNvx35x))
                return;

            Debug.Console(1, this , "Switching Video Input to : 'Hdmi2'");
            Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
        }

        public void SetVideoToNone()
        {
            Debug.Console(1, this, "Switching Video Input to : 'Disable'");
            Hardware.Control.VideoSource = eSfpVideoSourceTypes.Disable;
        }

        public void SetVideoToStream()
        {
            Debug.Console(1, this, "Switching Video Input to : 'Stream'");
            Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }
    }
}