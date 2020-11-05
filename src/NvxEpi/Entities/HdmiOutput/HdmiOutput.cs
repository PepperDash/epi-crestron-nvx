using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.HdmiOutput
{
    public class HdmiOutput : IHdmiOutput
    {
        private readonly INvxDevice _device;
        protected readonly DmNvxBaseClass _hardware;

        public HdmiOutput(INvx35XHardware device)
        {
            _device = device;
            _hardware = device.Hardware;
            Initialize(device.Hardware);
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

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
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

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public BoolFeedback DisabledByHdcp { get; private set; }

        public IntFeedback HorizontalResolution { get; private set; }

        private void Initialize(DmNvx35x hardware)
        {
            DisabledByHdcp = HdmiOutputDisabledFeedback.GetFeedback(hardware);
            HorizontalResolution = HorizontalResolutionFeedback.GetFeedback(hardware);
        }
    }

    public class VideowallHdmiOutput : HdmiOutput, IVideowallMode
    {
        public VideowallHdmiOutput(INvx35XHardware device) : base(device)
        {
            VideowallMode = VideowallModeFeedback.GetFeedback(device.Hardware);
        }

        public IntFeedback VideowallMode { get; private set; }

        public void SetVideowallMode(ushort value)
        {
            if (IsTransmitter)
                return;

            Debug.Console(1, this, "Setting videowall mode to : '{0}'", value);
            if (_hardware.HdmiOut != null)
                _hardware.HdmiOut.VideoWallMode.UShortValue = value;
        }
    }
}