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
}