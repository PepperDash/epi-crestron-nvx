using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class Hdmi1HdcpCapabilityFeedback
    {
        public const string Key = "Hdmi1HdcpCapability";

        private static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                throw new NotSupportedException("hdmi in 1");

            var feedback = new StringFeedback(Key,
                () => device.HdmiIn[1].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }

        public static StringFeedback GetFeedback(DmNvxE3x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }
    }
}