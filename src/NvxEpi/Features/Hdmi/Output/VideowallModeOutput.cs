using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Output;

public class VideowallModeOutput : HdmiOutput, IVideowallMode
{
    private readonly IntFeedback _videowallMode;
    private readonly IntFeedback _videoAspectRatioMode;

    public VideowallModeOutput(INvxDeviceWithHardware device) : base(device)
    {
        _videowallMode = VideowallModeFeedback.GetFeedback(device.Hardware);
        _videoAspectRatioMode = VideoAspectRatioModeFeedback.GetFeedback(device.Hardware);
        device.Feedbacks.Add(_videowallMode);
        device.Feedbacks.Add(_videoAspectRatioMode);

    }

    public IntFeedback VideowallMode { get { return _videowallMode; } }
    public IntFeedback VideoAspectRatioMode { get { return _videoAspectRatioMode; } }
}