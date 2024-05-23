using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

public class AnalogAudioInput
{
    public static void AddRoutingPort(INvxDevice device)
    {
        var port = new RoutingInputPort(
            DeviceInputEnum.AnalogAudio.Name,
            eRoutingSignalType.Audio,
            eRoutingPortConnectionType.LineAudio,
            DeviceInputEnum.AnalogAudio,
            device)
        {
            FeedbackMatchObject = eSfpAudioSourceTypes.Analog
        };

        device.InputPorts.Add(port);
    }
}