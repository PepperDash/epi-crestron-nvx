using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class UsbLocalAddressFeedback
    {
        public const string Key = "UsbLocalId";

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            var feedback = new StringFeedback(Key, () => device.UsbInput.LocalDeviceIdFeedback.StringValue);

            device.UsbInput.UsbInputChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}