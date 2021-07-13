using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DanteInputFeedback
    {
        public const string Key = "DanteInput";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(() => string.Empty);

            if (device.Control.DanteAes67Name != null)
            {
                feedback = new StringFeedback(Key, () => device.Control.ActiveDanteAudioSourceFeedback.ToString());
                device.BaseEvent += (@base, args) => feedback.FireUpdate();
            }
            
            return feedback;
        }
    }
}