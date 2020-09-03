using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models.Entities;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class NaxRxStatusFeedback
    {
        public static readonly string Key = "NaxRxStatus";

        public static StringFeedback GetNaxTxStatusFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback.ToString());

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}