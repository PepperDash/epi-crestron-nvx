using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using NvxEpi.Device.Services.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class DeviceName
    {
        public static StringFeedback GetDeviceNameFeedback(this DmNvxBaseClass device, DeviceConfig config)
        {
            device.UpdateName(config.Name);
            var props = NvxDeviceProperties.FromDeviceConfig(config);

            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.DeviceName.ToString(),
                () => String.IsNullOrEmpty(props.FriendlyName) ? device.Control.NameFeedback.StringValue : props.FriendlyName);

            device.BaseEvent += (@base, args) =>
            {
                if (args.EventId != DMInputEventIds.NameFeedbackEventId)
                    return;

                feedback.FireUpdate();
            };

            return feedback;
        }

        public static void UpdateName(this DmNvxBaseClass device, string name)
        {
            name = name.Replace(" ", "-");
            device.Control.Name.StringValue = name;
        }
    }
}