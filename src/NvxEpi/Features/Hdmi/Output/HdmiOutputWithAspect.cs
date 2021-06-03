using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Output
{
    public class HdmiOutputWithAspect : HdmiOutput
    {
        public HdmiOutputWithAspect(INvxDeviceWithHardware device)
            : base(device)
        {
            VideoAspectRatioMode = VideoAspectRatioModeFeedback.GetFeedback(device.Hardware);

            device.Feedbacks.Add(VideoAspectRatioModeFeedbackName.GetFeedback(device.Hardware));
        }

        public IntFeedback VideoAspectRatioMode { get; private set; }
    }
}