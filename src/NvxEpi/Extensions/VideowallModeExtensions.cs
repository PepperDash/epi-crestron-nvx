using NvxEpi.Abstractions.HdmiOutput;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class VideowallModeExtensions
    {
        public static void SetVideowallMode(this IVideowallMode device, ushort value)
        {
            if (device.IsTransmitter)
                return;

            Debug.Console(1, device, "Setting videowall mode to : '{0}'", value);
            if (device.Hardware.HdmiOut != null) 
                device.Hardware.HdmiOut.VideoWallMode.UShortValue = value;
        }
    }
}