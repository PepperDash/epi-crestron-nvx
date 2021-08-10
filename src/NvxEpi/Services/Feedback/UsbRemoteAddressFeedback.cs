using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class UsbRemoteAddressFeedback
    {
        public const string Key = "UsbRemoteId";

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            var feedback = new StringFeedback(Key, () => device.UsbInput.RemoteDeviceIdFeedback.StringValue);

            device.UsbInput.UsbInputChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}