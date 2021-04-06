using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HdmiOutputDisabledFeedback
    {
        public const string Key = "HdmiOutputDisabled";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiOut == null)
                return new BoolFeedback(Key, () => false);

            var feedback = new BoolFeedback(Key, () => device.HdmiOut.DisabledByHdcpFeedback.BoolValue);
            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}