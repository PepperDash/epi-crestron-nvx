using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DeviceHostnameFeedback
    {
        public const string Key = "DeviceHostname";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.Network.HostNameFeedback.StringValue);
            device.Network.NetworkChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}