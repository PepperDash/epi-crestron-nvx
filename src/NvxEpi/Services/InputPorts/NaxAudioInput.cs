using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class NaxAudioInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            var port = new RoutingInputPort(
                DeviceInputEnum.DmNaxAudio.Name,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.DmNaxAudio,
                device);

            device.InputPorts.Add(port);
        }
    }
}