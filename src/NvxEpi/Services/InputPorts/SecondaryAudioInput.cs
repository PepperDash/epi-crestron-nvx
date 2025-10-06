using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts;

public class SecondaryAudioInput
{
    public static void AddRoutingPort(INvxDevice device)
    {
        var port = new RoutingInputPort(
            DeviceInputEnum.SecondaryAudio.Name,
            eRoutingSignalType.Audio | eRoutingSignalType.SecondaryAudio,
            eRoutingPortConnectionType.Streaming,
            DeviceInputEnum.SecondaryAudio,
            device)
        {
            FeedbackMatchObject = DmNvxControl.eAudioSource.DmNaxAudio
        };

        device.InputPorts.Add(port);
    }
}