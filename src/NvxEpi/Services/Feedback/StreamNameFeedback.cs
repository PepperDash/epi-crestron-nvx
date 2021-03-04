using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class StreamNameFeedback
    {
        public const string Key = "StreamName";

        public static StringFeedback GetFeedback(string streamName)
        {
            return new StringFeedback(Key, () => streamName);
        }
    }
}