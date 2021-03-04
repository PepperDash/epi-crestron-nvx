using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DeviceModeNameFeedback
    {
        public const string Key = "DeviceMode";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.Control.DeviceModeFeedback.ToString());
            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}