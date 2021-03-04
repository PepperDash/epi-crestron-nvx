using System;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class StreamInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            if (device.IsTransmitter)
                throw new NotSupportedException("stream");

            var port = new RoutingInputPort(
                DeviceInputEnum.Stream.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.Stream,
                device);

            device.InputPorts.Add(port);
        }
    }
}