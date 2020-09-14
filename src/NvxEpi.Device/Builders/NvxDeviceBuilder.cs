using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Entities;
using NvxEpi.Device.Services.Config;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Builders
{
    public class NvxDeviceBuilder : INvxDeviceBuilder, IKeyed
    {
        private NvxDeviceBuilder(DeviceConfig config)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            var isTransmitter = props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase);

            IsTransmitter = new BoolFeedback("IsTransmitter", () => isTransmitter);
            Device = BuildDevice(config.Type, props);
            Config = config;
            VirtualDeviceId = props.VirtualDeviceId;
        }

        public static INvxDeviceBuilder Create(DeviceConfig config)
        {
            return new NvxDeviceBuilder(config);
        }

        public int VirtualDeviceId { get; private set; }
        public DeviceConfig Config { get; private set; }
        public DmNvxBaseClass Device { get; private set; }
        public BoolFeedback IsTransmitter { get; private set; }

        public INvxDevice Build()
        {
            return new NvxDevice(this);
        }

        private static DmNvxBaseClass BuildDevice(string type, NvxDeviceProperties props)
        {
            var nvxDeviceType = typeof(DmNvxBaseClass)
                .GetCType()
                .Assembly
                .GetTypes()
                .FirstOrDefault(x => x.Name.Equals(type, StringComparison.OrdinalIgnoreCase));

            if (nvxDeviceType == null)
                throw new NullReferenceException("The type specified in the config file wasn't found");

            if (props.Control.IpId == null)
                throw new Exception("The IPID for this device must be defined");

            return nvxDeviceType
                .GetConstructor(new CType[] { typeof(ushort).GetCType(), typeof(CrestronControlSystem) })
                .Invoke(new object[] { props.Control.IpIdInt, Global.ControlSystem }) as DmNvxBaseClass;
        }

        public string Key 
        {
            get { return Config.Key; }
        }
    }
}