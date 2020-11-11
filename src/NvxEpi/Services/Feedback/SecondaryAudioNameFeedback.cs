using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class SecondaryAudioNameFeedback
    {
        public const string Key = "SecondaryAudioName";

        public static StringFeedback GetFeedback(string secondaryAudioName)
        {
            return new StringFeedback(Key, () => secondaryAudioName);
        }
    }
}