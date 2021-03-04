using System;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class PrimaryAudioInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            if (device.IsTransmitter)
                throw new NotSupportedException("primary audio");

            var port = new RoutingInputPort(
                DeviceInputEnum.PrimaryAudio.Name,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.PrimaryAudio,
                device);

            device.InputPorts.Add(port);
        }
    }
}