using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class AudioInputValueFeedback
    {
        public const string Key = "AudioInputValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => (int)device.Control.ActiveAudioSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}