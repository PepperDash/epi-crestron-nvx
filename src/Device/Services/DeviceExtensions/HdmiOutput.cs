using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class HdmiOutput
    {
        public static IntFeedback GetHorizontalResolutionFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.HdmiOutputHorizontalResolution.ToString(),
                () => device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue);

            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static IntFeedback GetVideowallModeFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.VideowallMode.ToString(),
                () => device.HdmiOut.VideoWallModeFeedback.UShortValue);

            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }

        public static BoolFeedback GetHdmiOutputDisabledFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(NvxDevice.DeviceFeedbacks.HdmiOutputDisabledByHdcp.ToString(),
                () => device.HdmiOut.DisabledByHdcpFeedback.BoolValue);

            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}