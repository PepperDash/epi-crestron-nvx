using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.DeviceFeedback
{
    public class IsStreamingSecondaryAudio
    {
        public static readonly string Key = "IsStreamingSecondaryAudio";

        public static BoolFeedback GetFeedback(DmNvx35x device)
        {
            var feedback = new BoolFeedback(Key, () => device.SecondaryAudio.StartFeedback.BoolValue);
            device.SecondaryAudio.SecondaryAudioChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}