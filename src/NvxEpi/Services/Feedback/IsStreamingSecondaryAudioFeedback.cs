using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingSecondaryAudioFeedback
    {
        public static readonly string Key = "IsStreamingSecondaryAudio";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, () => device.SecondaryAudio.StartFeedback.BoolValue);
            device.SecondaryAudio.SecondaryAudioChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}