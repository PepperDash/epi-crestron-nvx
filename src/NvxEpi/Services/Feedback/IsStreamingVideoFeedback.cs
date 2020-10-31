using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingVideoFeedback
    {
        public const string Key = "IsStreamingVideo";

        public static BoolFeedback GetFeedbackForTx(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () => device.Control.ActiveVideoSourceFeedback == eSfpVideoSourceTypes.Stream && device.Control.StartFeedback.BoolValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static BoolFeedback GetFeedbackForRx(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () => device.Control.StartFeedback.BoolValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}