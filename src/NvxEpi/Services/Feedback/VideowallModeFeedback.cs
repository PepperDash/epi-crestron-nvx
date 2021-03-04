using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class VideowallModeFeedback
    {
        public const string Key = "VideowallMode";

        public static IntFeedback GetFeedback(DmNvx35x device)
        {
            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoWallModeFeedback.UShortValue);
            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}