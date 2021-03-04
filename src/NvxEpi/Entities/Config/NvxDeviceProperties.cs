using Crestron.SimplSharpPro.DeviceSupport;
using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Entities.Config
{
    public class NvxDeviceProperties
    {
        public static NvxDeviceProperties FromDeviceConfig(DeviceConfig config)
        {
            return JsonConvert.DeserializeObject<NvxDeviceProperties>(config.Properties.ToString());
        }

        public int DeviceId { get; set; }
        public ControlPropertiesConfig Control { get; set; }
        public NvxUsbProperties Usb { get; set; }
        public string Mode { get; set; }
        public string MulticastVideoAddress { get; set; }
        public string MulticastAudioAddress { get; set; } 
        public string VideoSourceName { get; set; }
        public string AudioSourceName { get; set; }
    }

    public class NvxUsbProperties
    {
        public string Mode { get; set; }
        public int UsbId { get; set; }
        public string Default { get; set; }
    }
}