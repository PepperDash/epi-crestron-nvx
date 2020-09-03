using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class VideowallFeedback
    {
        public static readonly string Key = "VideowallMode";
        public static IntFeedback GetVideowallModeFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoWallModeFeedback.UShortValue);

            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}