using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class Hdmi2HdcpCapabilityFeedback
    {
        public const string Key = "Hdmi2HdcpCapability";

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            if (device.HdmiIn == null || device.HdmiIn[2] == null)
                throw new NotSupportedException("hdmi in 2");

            var feedback = new StringFeedback(Key,
                () => device.HdmiIn[2].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}