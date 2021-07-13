using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class Nvx35XDeviceFactory : NvxBaseDeviceFactory<Nvx35X>
    {
        private static IEnumerable<string> _typeNames;

        public Nvx35XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

            if (_typeNames == null)
            {
                _typeNames = new List<string>
                {
                    "dmnvx350",
                    "dmnvx350c",
                    "dmnvx351",
                    "dmnvx351c",
                    "dmnvx352",
                    "dmnvx352c",
                };
            }

            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(dc);
            var deviceBuild = GetDeviceBuildAction(dc.Type, props);
            return new Nvx35X(dc, deviceBuild, props.DeviceIsTransmitter());
        }
    }
}