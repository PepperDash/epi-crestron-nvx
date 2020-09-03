using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class InputHdcpCapabilityFeedback
    {
        public static readonly string Hdmi1NameKey = "Hdmi1Capability";
        public static readonly string Hdmi1ValueKey = "Hdmi1CapabilityValue";
        public static readonly string Hdmi2NameKey = "Hdmi2Capability";
        public static readonly string Hdmi2ValueKey = "Hdmi2CapabilityValue";

        public static StringFeedback GetHdmiIn1HdcpCapabilityFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Hdmi1NameKey,
                () => device.HdmiIn[1].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static StringFeedback GetHdmiIn2HdcpCapabilityFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Hdmi2NameKey,
                () => device.HdmiIn[2].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetHdmiIn1HdcpCapabilityValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Hdmi1ValueKey,
                () => (int) device.HdmiIn[1].HdcpCapabilityFeedback);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetHdmiIn2HdcpCapabilityValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Hdmi2ValueKey,
                () => (int) device.HdmiIn[2].HdcpCapabilityFeedback);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}