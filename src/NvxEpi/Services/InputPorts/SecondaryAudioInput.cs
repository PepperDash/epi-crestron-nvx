﻿using System;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class SecondaryAudioInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            if (device.IsTransmitter)
                throw new NotSupportedException("secondary audio");

            var port = new RoutingInputPort(
                DeviceInputEnum.SecondaryAudio.Name,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.SecondaryAudio,
                device);

            device.InputPorts.Add(port);
        }
    }
}