using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class InputHdcpCapability
    {
        public static StringFeedback GetHdmiIn1HdcpCapabilityFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.Hdmi1HdcpCapabilityName.ToString(), 
                () => device.HdmiIn[1].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static StringFeedback GetHdmiIn2HdcpCapabilityFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.Hdmi2HdcpCapabilityName.ToString(),
                   () => device.HdmiIn[2].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetHdmiIn1HdcpCapabilityValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.Hdmi1HdcpCapabilityValue.ToString(),
                () => (int) device.HdmiIn[1].HdcpCapabilityFeedback);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetHdmiIn2HdcpCapabilityValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.Hdmi2HdcpCapabilityValue.ToString(),
                   () => (int) device.HdmiIn[2].HdcpCapabilityFeedback);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}