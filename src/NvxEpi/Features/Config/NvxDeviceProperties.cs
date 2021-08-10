using System;
using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Features.Config
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
        public string StreamUrl { get; set; }
        public string MulticastVideoAddress { get; set; }
        public string MulticastAudioAddress { get; set; }
        public string ParentDeviceKey { get; set; }
        public uint DomainId { get; set; }
        public string DefaultAudioInput { get; set; }
        public string DefaultVideoInput { get; set; }
    }

    public class NvxMockDeviceProperties
    {
        public int DeviceId { get; set; }
        public string StreamUrl { get; set; }
        public string MulticastVideoAddress { get; set; }
        public string MulticastAudioAddress { get; set; }
    }

    internal static class NvxDevicePropertiesExt
    {
        public static bool DeviceIsTransmitter(this NvxDeviceProperties props)
        {
            return !string.IsNullOrEmpty(props.Mode) &&
                        props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);
        }
    }

    public class NvxUsbProperties
    {
        public string Mode { get; set; }
        public string Default { get; set; }
        public bool FollowVideo { get; set; }
    }
}