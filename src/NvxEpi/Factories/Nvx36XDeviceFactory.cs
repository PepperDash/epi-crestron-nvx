using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class Nvx36XDeviceFactory : NvxBaseDeviceFactory<Nvx36X>
    {
        private static IEnumerable<string> _typeNames;

        public Nvx36XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

            if (_typeNames == null)
            {
                _typeNames = new List<string>
                {
                    "dmnvx360",
                    "dmnvx360c",
                    "dmnvx363",
                    "dmnvx363c",
                    "dmnvxe760",
                    "dmnvxe760c",
                };
            }

            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(dc);
            var deviceBuild = GetDeviceBuildAction(dc.Type, props);
            return new Nvx36X(dc, deviceBuild, props.DeviceIsTransmitter());
        }
    }
}