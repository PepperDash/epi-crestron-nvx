using System.Collections.Generic;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class DeviceStatus
    {
        public static StringFeedback GetDeviceStatusFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.DeviceStatus.ToString(),
                () => device.Control.StatusTextFeedback.StringValue);

            device.BaseEvent += (@base, args) =>
            {
                if (args.EventId != DMInputEventIds.StatusTextEventId)
                    return;

                feedback.FireUpdate();
            };

            return feedback;
        }
    }
}