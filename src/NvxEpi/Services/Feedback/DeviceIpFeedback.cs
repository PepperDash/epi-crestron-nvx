using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DeviceIpFeedback
    {
        public const string Key = "DeviceIp";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.Network.IpAddressFeedback.StringValue);
            device.Network.NetworkChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}