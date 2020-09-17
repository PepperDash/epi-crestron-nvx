using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceFeedback
{
    public class AudioInput
    {
        public const string Key = "AudioInput";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.Control.ActiveAudioSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}