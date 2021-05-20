using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Features.Config;
using NvxEpi.Features.Routing;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public abstract class NvxBaseDeviceFactory<T> : EssentialsPluginDeviceFactory<T> where T : EssentialsDevice
    {
        public const string MinumumEssentialsVersion = "1.8.0";

        private static IDictionary<string, Func<uint, DmNvxBaseClass>> _types;

        static NvxBaseDeviceFactory()
        {
            if (DeviceManager.GetDeviceForKey(NvxGlobalRouter.InstanceKey) == null)
                DeviceManager.AddDevice(NvxGlobalRouter.Instance);

            _types = new Dictionary<string, Func<uint, DmNvxBaseClass>>(StringComparer.OrdinalIgnoreCase)
            {
                {"dmnvx350", ipid => new DmNvx350(ipid, Global.ControlSystem)},
                {"dmnvx350c", ipid => new DmNvx350C(ipid, Global.ControlSystem)},
                {"dmnvx351", ipid => new DmNvx351(ipid, Global.ControlSystem)},
                {"dmnvx351c", ipid => new DmNvx351C(ipid, Global.ControlSystem)},
                {"dmnvx352", ipid => new DmNvx352(ipid, Global.ControlSystem)},
                {"dmnvx352c", ipid => new DmNvx352C(ipid, Global.ControlSystem)},
                {"dmnvx360", ipid => new DmNvx360(ipid, Global.ControlSystem)},
                {"dmnvx360c", ipid => new DmNvx360C(ipid, Global.ControlSystem)},
                {"dmnvx363", ipid => new DmNvx363(ipid, Global.ControlSystem)},
                {"dmnvx363c", ipid => new DmNvx363C(ipid, Global.ControlSystem)},
                {"dmnvxe30", ipid => new DmNvxE30(ipid, Global.ControlSystem)},
                {"dmnvxe30c", ipid => new DmNvxE30C(ipid, Global.ControlSystem)},
                {"dmnvxe31", ipid => new DmNvxE31(ipid, Global.ControlSystem)},
                {"dmnvxe31c", ipid => new DmNvxE31C(ipid, Global.ControlSystem)},
                {"dmnvxd30", ipid => new DmNvxD30(ipid, Global.ControlSystem)},
                {"dmnvxd30c", ipid => new DmNvxD30C(ipid, Global.ControlSystem)},
            };
        }

        protected static DmNvxBaseClass BuildDeviceFromConfig(DeviceConfig config)
        {

            var type = config.Type;
            var props = NvxDeviceProperties.FromDeviceConfig(config);

            Func<uint, DmNvxBaseClass> nvxDeviceType;
            if (!_types.TryGetValue(type, out nvxDeviceType))
            {
                Debug.Console(0, Debug.ErrorLogLevel.Warning, "The type specified '{0}' in the config file wasn't found", config.Type);
                return null;
            }

            if (props.Control.IpId == null)
                throw new Exception("The IPID for this device must be defined");

            return nvxDeviceType(props.Control.IpIdInt);
        }
    }
}