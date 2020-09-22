namespace NvxEpi.Abstractions.HdmiOutput
{
    public static class VideowallModeExtensions
    {
        public static void SetVideowallMode(this IVideowallMode device, ushort value)
        {
            if (device.IsTransmitter)
                return;

            if (device.Hardware.HdmiOut != null) 
                device.Hardware.HdmiOut.VideoWallMode.UShortValue = value;
        }
    }
}