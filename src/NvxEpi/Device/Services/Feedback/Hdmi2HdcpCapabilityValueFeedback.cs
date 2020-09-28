using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.Feedback
{
    public class Hdmi2HdcpCapabilityValueFeedback
    {
        public const string Key = "Hdmi2HdcpCapabilityValue";

        public static IntFeedback GetFeedback(DmNvx35x device)
        {
            if (device.HdmiIn == null || device.HdmiIn[2] == null)
                throw new NotSupportedException("hdmi in 2");

            var feedback = new IntFeedback(Key,
                () => (int)device.HdmiIn[2].HdcpCapabilityFeedback);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}