using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HorizontalResolutionFeedback
    {
        public const string Key = "HdmiOutResolution";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiOut == null)
                return new IntFeedback(Key, () => 0);

            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue);
            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}