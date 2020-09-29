using System;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class HdmiInput1
    {
        public static void AddRoutingPort(ICurrentVideoInput device)
        {
            if (device.Hardware.HdmiIn == null || device.Hardware.HdmiIn[1] == null)
                throw new NotSupportedException("hdmi 1");

            var port = new RoutingInputPort(
                DeviceInputEnum.Hdmi1.Name, 
                eRoutingSignalType.AudioVideo, 
                eRoutingPortConnectionType.Hdmi, 
                DeviceInputEnum.Hdmi1, 
                device);

            device.InputPorts.Add(port);
        }
    }
}