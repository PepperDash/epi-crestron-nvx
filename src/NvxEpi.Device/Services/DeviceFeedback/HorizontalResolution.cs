using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.DeviceFeedback
{
    public class HorizontalResolution
    {
        public const string Key = "HdmiOutputResolution";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue);
            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}