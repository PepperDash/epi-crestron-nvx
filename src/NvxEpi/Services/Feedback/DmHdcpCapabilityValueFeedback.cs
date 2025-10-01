using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class DmHdcpCapabilityValueFeedback
{
    public static IntFeedback GetFeedback(DmNvxBaseClass device)
    {
        if (device is not DmNvxE760x dmDevice)
            return new IntFeedback(Hdmi1HdcpCapabilityValueFeedback.Key, () => 0);

        var feedback = new IntFeedback(Hdmi1HdcpCapabilityValueFeedback.Key,
            () => (int)device.DmIn.HdcpCapability);

        device.DmIn.InputStreamChange += (stream, args) => feedback.FireUpdate();
        device.DmIn.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}

public class DmHdcpCapabilityStateFeedback
{
    public static IntFeedback GetFeedback(DmNvxBaseClass device)
    {
        if (device is not DmNvxE760x dmDevice)
            return new IntFeedback(Hdmi1HdcpCapabilityValueFeedback.Key, () => 0);

        var feedback = new IntFeedback(Hdmi1HdcpCapabilityValueFeedback.Key,
            () => (int)device.DmIn.VideoAttributes.HdcpStateFeedback);

        device.DmIn.InputStreamChange += (stream, args) => feedback.FireUpdate();
        device.DmIn.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}