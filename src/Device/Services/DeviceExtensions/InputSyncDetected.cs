using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class InputSyncDetected
    {
        public static BoolFeedback GetHdmiIn1SyncDetectedFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(NvxDevice.DeviceFeedbacks.Hdmi1SyncDetected.ToString(), 
                () => device.HdmiIn[1].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static BoolFeedback GetHdmiIn2SyncDetectedFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(NvxDevice.DeviceFeedbacks.Hdmi2SyncDetected.ToString(), 
                () => device.HdmiIn[2].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}