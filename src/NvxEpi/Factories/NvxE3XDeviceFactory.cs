using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class NvxE3XDeviceFactory : NvxBaseDeviceFactory<NvxE3X>
    {
        private static List<string> _typeNames;

        public NvxE3XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

            if (_typeNames == null)
            {
                _typeNames = new List<string>
                {
                    "dmnvxe30",
                    "dmnvxe30c",
                    "dmnvxe31",
                    "dmnvxe31c",
                };
            }

            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(dc);
            var deviceBuild = GetDeviceBuildAction(dc.Type, props);
            return new NvxE3X(dc, deviceBuild);
        }
    }
}