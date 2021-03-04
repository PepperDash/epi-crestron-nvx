using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class VideoInputValueFeedback
    {
        public const string Key = "VideoInputValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key,
                () => (int)device.Control.ActiveVideoSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}