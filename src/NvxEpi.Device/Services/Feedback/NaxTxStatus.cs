using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceFeedback
{
    public class NaxTxStatus
    {
        public static readonly string Key = "NaxTxStatus";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback.ToString());

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}