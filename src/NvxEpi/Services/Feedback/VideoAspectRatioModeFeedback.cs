using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class VideoAspectRatioModeFeedback
    {
        public const string Key = "VideoAspectRatioMode";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoAttributes.AspectRatioFeedback.UShortValue);
            device.HdmiOut.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}