using NvxEpi.Abstractions;
using NvxEpi.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputPorts
{
    public class AutomaticInput
    {
        public static void AddRoutingPort(INvxDevice device)
        {
            var port = new RoutingInputPort(
                DeviceInputEnum.Automatic.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.Automatic,
                device);

            device.InputPorts.Add(port);
        }
    }
}