using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using System;
using System.Collections.Generic;

namespace NvxEpi
{
    public class NvxDeviceFactory : EssentialsPluginDeviceFactory<NvxDevice>
    {
        public NvxDeviceFactory() => TypeNames = new List<string>
            {
                "dmnvx363",
                "dmnvx363c",
                "dmnvx384",
                "dmnvx384c",
                "dmnvx360",
                "dmnvx360c",
                "dmnvx350",
                "dmnvx350c",
                "dmnvx351",
                "dmnvx351c",
                "dmnvx352",
                "dmnvx352c",
                "dmnvxe30",
                "dmnvxe30c",
                "dmnvxe31",
                "dmnvxe31c",
                "dmnvxe10",
                "dmnvxe20",
                "dmnvxe202g",
                "dmnvxe760",
                "dmnvxe760c",
                "dmnvxd30",
                "dmnvxd30c",
            };

        internal static NvxDevice BuildDevice(string key, string type, NvxDeviceProperties props, DmXioDirectorBase.DmXioDomain? domain = null)
        {
            var ipid = domain is null ? props.Control?.IpIdInt ?? throw new ArgumentNullException(nameof(props.Control)) : (uint) props.DeviceId;
            var isReceiver = props.Mode.ToLowerInvariant() == "rx";

            DmNvxBaseClass device = type.ToLowerInvariant() switch
            {
                "dmnvx363" => domain == null
                    ? new DmNvx363(ipid, Global.ControlSystem)
                    : new DmNvx363(ipid, domain, isReceiver),
                "dmnvx363c" => domain == null
                    ? new DmNvx363C(ipid, Global.ControlSystem)
                    : new DmNvx363C(ipid, domain, isReceiver),
                "dmnvx384" => domain == null
                    ? new DmNvx384(ipid, Global.ControlSystem)
                    : new DmNvx384(ipid, domain, isReceiver),
                "dmnvx384c" => domain == null
                    ? new DmNvx384C(ipid, Global.ControlSystem)
                    : new DmNvx384C(ipid, domain, isReceiver),
                "dmnvx360" => domain == null
                    ? new DmNvx360(ipid, Global.ControlSystem)
                    : new DmNvx360(ipid, domain, isReceiver),
                "dmnvx360c" => domain == null
                    ? new DmNvx360C(ipid, Global.ControlSystem)
                    : new DmNvx360C(ipid, domain, isReceiver),
                "dmnvx350" => domain == null
                    ? new DmNvx350(ipid, Global.ControlSystem)
                    : new DmNvx350(ipid, domain, isReceiver),
                "dmnvx350c" => domain == null
                    ? new DmNvx350C(ipid, Global.ControlSystem)
                    : new DmNvx350C(ipid, domain, isReceiver),
                "dmnvx351" => domain == null
                    ? new DmNvx351(ipid, Global.ControlSystem)
                    : new DmNvx351(ipid, domain, isReceiver),
                "dmnvx351c" => domain == null
                    ? new DmNvx351C(ipid, Global.ControlSystem)
                    : new DmNvx351C(ipid, domain, isReceiver),
                "dmnvx352" => domain == null
                    ? new DmNvx352(ipid, Global.ControlSystem)
                    : new DmNvx352(ipid, domain, isReceiver),
                "dmnvx352c" => domain == null
                    ? new DmNvx352C(ipid, Global.ControlSystem)
                    : new DmNvx352C(ipid, domain, isReceiver),
                "dmnvxe30" => domain == null
                    ? new DmNvxE30(ipid, Global.ControlSystem)
                    : new DmNvxE30(ipid, domain),
                "dmnvxe30c" => domain == null
                    ? new DmNvxE30C(ipid, Global.ControlSystem)
                    : new DmNvxE30C(ipid, domain),
                "dmnvxe31" => domain == null
                    ? new DmNvxE31(ipid, Global.ControlSystem)
                    : new DmNvxE31(ipid, domain),
                "dmnvxe31c" => domain == null
                    ? new DmNvxE31C(ipid, Global.ControlSystem)
                    : new DmNvxE31C(ipid, domain),
                "dmnvxe10" => domain == null
                    ? new DmNvxE10(ipid, Global.ControlSystem)
                    : new DmNvxE10(ipid, domain),
                "dmnvxe20" => domain == null
                    ? new DmNvxE20(ipid, Global.ControlSystem)
                    : new DmNvxE20(ipid, domain),
                "dmnvxe202g" => domain == null
                    ? new DmNvxE202g(ipid, Global.ControlSystem)
                    : new DmNvxE202g(ipid, domain),
                "dmnvxe760" => domain == null
                    ? new DmNvxE760(ipid, Global.ControlSystem)
                    : new DmNvxE760(ipid, domain),
                "dmnvxe760c" => domain == null
                    ? new DmNvxE760C(ipid, Global.ControlSystem)
                    : new DmNvxE760C(ipid, domain),
                "dmnvxd30" => domain == null
                    ? new DmNvxD30(ipid, Global.ControlSystem)
                    : new DmNvxD30(ipid, domain),
                "dmnvxd30c" => domain == null
                    ? new DmNvxD30C(ipid, Global.ControlSystem)
                    : new DmNvxD30C(ipid, domain),
                _ => throw new NotSupportedException(type),
            };

            return new NvxDevice(key, props, device);
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var props = dc.Properties.ToObject<NvxDeviceProperties>() ?? throw new ArgumentException("Invalid device configuration", nameof(dc));
            return BuildDevice(dc.Key, dc.Type, props);
        }
    }

    public class MockNvxDeviceFactory : EssentialsPluginDeviceFactory<NvxMockDevice>
    {
        public MockNvxDeviceFactory() => TypeNames = ["mockNvxDevice"];

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var props = dc.Properties.ToObject<NvxDeviceProperties>() ?? throw new ArgumentException("Invalid device configuration", nameof(dc));

            return new NvxMockDevice(
                dc.Key,
                dc.Name,
                props.StreamUrl,
                props.MulticastAddress,
                props.DmNaxTransmitAddress,
                props.DmNaxReceiveAddress,
                props.DeviceId,
                props.Mode.ToLowerInvariant() == "tx");
        }
    }
}
