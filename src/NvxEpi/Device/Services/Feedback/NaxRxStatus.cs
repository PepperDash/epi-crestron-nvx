using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceFeedback
{
    public class NaxRxStatus
    {
        public static readonly string Key = "NaxRxStatus";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback.ToString());

            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}