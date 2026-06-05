using PepperDash.Core;

namespace NvxEpi
{
    public class NvxDeviceProperties
    {
        public int DeviceId { get; set; }
        public ControlPropertiesConfig? Control { get; set; }
        public string Mode { get; set; } = "rx";
        public string StreamUrl { get; set; } = "";
        public string MulticastAddress { get; set; } = "";
        public string DmNaxTransmitAddress { get; set; } = "";
        public string DmNaxReceiveAddress { get; set; } = "";
    }
}
