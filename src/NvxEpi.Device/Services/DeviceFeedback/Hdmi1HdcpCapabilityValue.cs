using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceFeedback
{
    public class Hdmi1HdcpCapabilityValue
    {
        public const string Key = "Hdmi1HdcpCapabilityValue";

        private static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                throw new NotSupportedException("hdmi in 1");

            var feedback = new IntFeedback(Key,
                () => (int)device.HdmiIn[1].HdcpCapabilityFeedback);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetFeedback(DmNvx35x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }

        public static IntFeedback GetFeedback(DmNvxE3x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }
    }
}