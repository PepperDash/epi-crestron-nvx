using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HorizontalResolutionFeedback
    {
        public const string Key = "HdmiOutputResolution";

        private static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue);
            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static IntFeedback GetFeedback(DmNvx35x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }

        public static IntFeedback GetFeedback(DmNvxD3x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }
    }
}