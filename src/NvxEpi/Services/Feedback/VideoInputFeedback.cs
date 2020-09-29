using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class VideoInputFeedback
    {
        public const string Key = "VideoInput";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.Control.ActiveVideoSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        } 
    }
}