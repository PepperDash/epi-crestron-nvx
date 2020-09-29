using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class VideoStreamStatusFeedback
    {
        public const string Key = "VideoStreamStatus";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.Control.StatusTextFeedback.StringValue);

            device.BaseEvent += (@base, args) =>
            {
                if (args.EventId != DMInputEventIds.StatusTextEventId)
                    return;

                feedback.FireUpdate();
            };

            return feedback;
        }      
    }
}