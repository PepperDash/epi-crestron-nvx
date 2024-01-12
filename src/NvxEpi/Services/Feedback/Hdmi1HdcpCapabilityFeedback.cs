using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class Hdmi1HdcpCapabilityFeedback
    {
        public const string Key = "Hdmi1HdcpCapability";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                return new StringFeedback(() => String.Empty);

            var feedback = new StringFeedback(Key,
                () => device.HdmiIn[1].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class Hdmi1HdcpStateFeedback
    {
        public const string Key = "Hdmi1HdcpState";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(Key,
                () => (int)device.HdmiIn[1].VideoAttributes.HdcpStateFeedback);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}