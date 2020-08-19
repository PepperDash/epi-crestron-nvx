﻿using System.Collections.Generic;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class DeviceMode
    {
        public static IntFeedback GetDeviceModeFeedback(this DmNvxBaseClass device)
        {
            if (device is DmNvxD3x)
            {
                return new IntFeedback(NvxDevice.DeviceFeedbacks.DeviceMode.ToString(),
                    () => DeviceModeEnum.Receiver.Value);
            }

            if (device is DmNvxE3x)
            {
                return new IntFeedback(NvxDevice.DeviceFeedbacks.DeviceMode.ToString(),
                    () => DeviceModeEnum.Transmitter.Value);
            }

            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.DeviceMode.ToString(),
                () => (int) device.Control.DeviceModeFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static DmNvxBaseClass SetDeviceMode(this DmNvxBaseClass device, DeviceModeEnum mode)
        {
            return mode == DeviceModeEnum.Transmitter ? device.IsTransmitter() : device.IsReceiver();
        }

        public static DmNvxBaseClass IsTransmitter(this DmNvxBaseClass device)
        {
            if (device is DmNvxD3x)
                return device;

            device.Control.DeviceMode = eDeviceMode.Transmitter;
            return device;
        }

        public static DmNvxBaseClass IsReceiver(this DmNvxBaseClass device)
        {
            if (device is DmNvxE3x)
                return device;

            device.Control.DeviceMode = eDeviceMode.Receiver;
            return device;
        }
    }
}