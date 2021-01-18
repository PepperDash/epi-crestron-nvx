using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class Hdmi2HdcpCapabilityValueFeedback
    {
        public const string Key = "Hdmi2HdcpCapabilityValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[2] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(Key,
                () => (int)device.HdmiIn[2].HdcpCapabilityFeedback);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}