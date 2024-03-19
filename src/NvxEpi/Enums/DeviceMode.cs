
using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class DeviceModeEnum : Enumeration<DeviceModeEnum>
    {
        private DeviceModeEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly DeviceModeEnum Receiver = new DeviceModeEnum(0, "Receiver");
        public static readonly DeviceModeEnum Transmitter = new DeviceModeEnum(1, "Transmitter");
    }
}