using System;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Entities.Config;
using NvxEpi.Device.Entities.Routing;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Factories
{
    public abstract class NvxBaseDeviceFactory<T> : EssentialsPluginDeviceFactory<T> where T : EssentialsDevice
    {
        static NvxBaseDeviceFactory()
        {
            if (DeviceManager.GetDeviceForKey(NvxDeviceRouter.InstanceKey) == null)
                DeviceManager.AddDevice(NvxDeviceRouter.Instance);
        }

        public static DmNvxBaseClass BuildDeviceFromConfig(DeviceConfig config)
        {
            var type = config.Type;
            var props = NvxDeviceProperties.FromDeviceConfig(config);

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
    }
}