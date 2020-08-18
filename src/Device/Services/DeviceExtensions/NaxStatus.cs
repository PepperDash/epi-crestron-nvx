using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class NaxStatus
    {
        public static StringFeedback GetNaxRxStatusFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.NaxRxStatus.ToString(),
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback.ToString());

            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static StringFeedback GetNaxTxStatusFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.NaxTxStatus.ToString(),
                () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback.ToString());

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}