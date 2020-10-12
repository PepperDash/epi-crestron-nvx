using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DeviceNameFeedback
    {
        public const string Key = "DeviceName";

        public static StringFeedback GetFeedback(string deviceName)
        {
            return new StringFeedback(Key, () => deviceName);
        }
    }

    public class StreamNameFeedback
    {
        public const string Key = "StreamName";

        public static StringFeedback GetFeedback(string streamName)
        {
            return new StringFeedback(Key, () => streamName);
        }
    }

    public class SecondaryAudioNameFeedback
    {
        public const string Key = "SecondaryAudioName";

        public static StringFeedback GetFeedback(string secondaryAudioName)
        {
            return new StringFeedback(Key, () => secondaryAudioName);
        }
    }
}