using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HdmiOutputDisabledFeedback
    {
        public const string Key = "HdmiOutputDisabled";

        private static BoolFeedback GetFeedback(Dm100xStrBase device)
        {
            if (device.HdmiOut == null)
                throw new NotSupportedException("hdmi out");

            var feedback = new BoolFeedback(Key, () => device.HdmiOut.DisabledByHdcpFeedback.BoolValue);
            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }

        public static BoolFeedback GetFeedback(DmNvx35x device)
        {
            return GetFeedback(device as Dm100xStrBase);
        }

        public static BoolFeedback GetFeedback(DmNvxD3x device)
        {
            return GetFeedback(device as Dm100xStrBase);
        }
    }
}