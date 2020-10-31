﻿using System;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class HdmiInput2Port
    {
        public static void AddRoutingPort(ICurrentVideoInput device)
        {
            if (device.Hardware.HdmiIn == null || device.Hardware.HdmiIn[2] == null)
                throw new NotSupportedException("hdmi 2");

            var port = new RoutingInputPort(
                DeviceInputEnum.Hdmi2.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                DeviceInputEnum.Hdmi2,
                device);

            device.InputPorts.Add(port);
        }
    }
}