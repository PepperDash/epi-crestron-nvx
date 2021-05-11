using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.Routing;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public abstract class NvxBaseDeviceFactory<T> : EssentialsPluginDeviceFactory<T> where T : EssentialsDevice
    {
        public const string MinumumEssentialsVersion = "1.8.0";

        private static IDictionary<string, CType> _types;

        static NvxBaseDeviceFactory()
        {
            if (DeviceManager.GetDeviceForKey(NvxGlobalRouter.InstanceKey) == null)
                DeviceManager.AddDevice(NvxGlobalRouter.Instance); 
        }

        protected static DmNvxBaseClass BuildDeviceFromConfig(DeviceConfig config)
        {
            if (_types == null)
            {
                _types = typeof(DmNvxBaseClass)
                    .GetCType()
                    .Assembly
                    .GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(DmNvxBaseClass).GetCType()) && !x.IsAbstract)
                    .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
            }

            var type = config.Type;
            var props = NvxDeviceProperties.FromDeviceConfig(config);

            CType nvxDeviceType;
            if (!_types.TryGetValue(type, out nvxDeviceType))
                throw new NullReferenceException("The type specified in the config file wasn't found");

            if (props.Control.IpId == null)
                throw new Exception("The IPID for this device must be defined");

            return nvxDeviceType
                .GetConstructor(new CType[] { typeof(ushort).GetCType(), typeof(CrestronControlSystem) })
                .Invoke(new object[] { props.Control.IpIdInt, Global.ControlSystem }) as DmNvxBaseClass;
        }
    }
}