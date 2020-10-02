using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class StreamUrlFeedback
    {
        public const string Key = "StreamUrl";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.Control.ServerUrlFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SourceReceive.StreamChange += (stream, args) => feedback.FireUpdate();
            device.SourceTransmit.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}