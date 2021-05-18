using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Output
{
    public class VideowallModeOutput : HdmiOutput, IVideowallMode
    {
        private readonly IntFeedback _videowallMode;

        public VideowallModeOutput(INvxDeviceWithHardware device) : base(device)
        {
            _videowallMode = VideowallModeFeedback.GetFeedback(device.Hardware);
            device.Feedbacks.Add(_videowallMode);
        }

        public IntFeedback VideowallMode { get { return _videowallMode; } }
    }
}