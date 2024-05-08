
using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class HdcpCapabilityEnum : Enumeration<HdcpCapabilityEnum>
    {
        private HdcpCapabilityEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly HdcpCapabilityEnum HdcpOff = new HdcpCapabilityEnum(0, "Hdcp Off");
        public static readonly HdcpCapabilityEnum Auto = new HdcpCapabilityEnum(1, "Hdcp Auto");
        public static readonly HdcpCapabilityEnum Hdcp1x = new HdcpCapabilityEnum(2, "Hdcp 1.x");
        public static readonly HdcpCapabilityEnum Hdcp2 = new HdcpCapabilityEnum(3, "Hdcp 2.2");
    }
}