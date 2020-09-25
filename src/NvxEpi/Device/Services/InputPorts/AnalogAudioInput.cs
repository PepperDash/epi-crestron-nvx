using System;
using NvxEpi.Abstractions;
using NvxEpi.Device.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.InputPorts
{
    public class AnalogAudioInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            if (!device.IsTransmitter)
                throw new NotSupportedException("analog audio");

            var port = new RoutingInputPort(
                DeviceInputEnum.AnalogAudio.Name,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.LineAudio,
                DeviceInputEnum.AnalogAudio,
                device);

            device.InputPorts.Add(port);
        }
    }
}