using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class NaxInputValueFeedback
    {
        public const string Key = "NaxInputValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => (int)device.Control.ActiveDmNaxAudioSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}