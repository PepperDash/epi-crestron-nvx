using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class MulticastAddressFeedback
    {
        public const string Key = "MulticastAddress";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.Control.MulticastAddressFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}