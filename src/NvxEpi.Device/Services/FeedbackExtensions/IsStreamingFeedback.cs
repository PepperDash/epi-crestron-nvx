using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class IsStreamingFeedback
    {
        public static readonly string Key = "IsStreaming";
        public static BoolFeedback GetIsStreamingFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, () => device.Control.StartFeedback.BoolValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}