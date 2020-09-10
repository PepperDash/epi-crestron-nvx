using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class StreamUrlFeedback
    {
        public static readonly string Key = "StreamUrl";
        public static StringFeedback GetStreamUrlFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.Control.ServerUrlFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}