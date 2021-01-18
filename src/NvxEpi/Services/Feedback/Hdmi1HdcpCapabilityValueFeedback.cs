using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class Hdmi1HdcpCapabilityValueFeedback
    {
        public const string Key = "Hdmi1HdcpCapabilityValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(Key,
                () => (int)device.HdmiIn[1].HdcpCapabilityFeedback);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}