using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DanteInputValueFeedback
    {
        public const string Key = "DanteInputValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(() => 0);

            if (device.Control.DanteAes67Name != null)
            {
                feedback = new IntFeedback(Key, () => (int)device.Control.DanteAudioSourceFeedback);
                device.BaseEvent += (@base, args) => feedback.FireUpdate();
            }

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}