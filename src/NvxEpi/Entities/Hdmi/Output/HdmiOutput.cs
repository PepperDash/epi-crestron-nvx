using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Output
{
    public class HdmiOutput : IHdmiOutput
    {
        private readonly INvxDeviceWithHardware _device;
        private readonly BoolFeedback _disabledByHdcp;
        private readonly IntFeedback _horizontalResolution;
        private readonly IntFeedback _videoAspectRatioMode;

        public HdmiOutput(INvxDeviceWithHardware device)
        {
            _device = device;

            _disabledByHdcp = HdmiOutputDisabledFeedback.GetFeedback(device.Hardware);
            _horizontalResolution = HorizontalResolutionFeedback.GetFeedback(device.Hardware);
            _videoAspectRatioMode = VideoAspectRatioModeFeedback.GetFeedback(device.Hardware);

            device.Feedbacks.Add(DisabledByHdcp);
            device.Feedbacks.Add(HorizontalResolution);
            device.Feedbacks.Add(VideoAspectRatioMode);
            device.Feedbacks.Add(VideoAspectRatioModeFeedbackName.GetFeedback(device.Hardware));
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _disabledByHdcp; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public IntFeedback HorizontalResolution
        {
            get { return _horizontalResolution; }
        }

        public IntFeedback VideoAspectRatioMode
        {
            get { return _videoAspectRatioMode; }
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