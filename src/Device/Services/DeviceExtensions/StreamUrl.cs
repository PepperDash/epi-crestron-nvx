using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class StreamUrl
    {
        public static StringFeedback GetStreamUrlFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.StreamUrl.ToString(),
                () => device.Control.ServerUrlFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}