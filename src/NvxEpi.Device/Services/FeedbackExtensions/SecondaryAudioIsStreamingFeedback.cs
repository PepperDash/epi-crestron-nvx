using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class SecondaryAudioIsStreamingFeedback
    {
        public static readonly string Key = "IsStreaming";
        public static BoolFeedback GetSecondaryAudioIsStreamingFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, () => device.SecondaryAudio.StartFeedback.BoolValue);

            device.SecondaryAudio.SecondaryAudioChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}