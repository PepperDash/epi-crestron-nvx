using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class NoSwitchInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            var port = new RoutingInputPort(
                DeviceInputEnum.NoSwitch.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.NoSwitch,
                device);

            device.InputPorts.Add(port);
        }
    }
}