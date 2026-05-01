using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class DmCurrentResolutionFeedback
{
    public const string Key = "DmInCurrentResolution";

    public static StringFeedback GetFeedback(DmNvxBaseClass device)
    {
        if (device is not DmNvxE760x)
            return new StringFeedback(Key, () => string.Empty);

        var feedback = new StringFeedback(Key, () =>
            string.Format("{0}x{1}@{2}",
                device.DmIn.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                device.DmIn.VideoAttributes.VerticalResolutionFeedback.UShortValue,
                device.DmIn.VideoAttributes.FramesPerSecondFeedback.UShortValue));

        device.DmIn.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}
