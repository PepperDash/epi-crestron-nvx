using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class HdmiOutputDisabledFeedback
    {
        public static readonly string Key = "HdmiOutputDisabled";
        public static BoolFeedback GetHdmiOutputDisabledFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, () => device.HdmiOut.DisabledByHdcpFeedback.BoolValue);

            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}