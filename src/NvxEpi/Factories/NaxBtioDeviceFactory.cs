// C#
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
    {
    public class NaxBtioDeviceFactory : NvxBaseDeviceFactory<DmNaxBtioDevice>
        {
        private static readonly List<string> _typeNames = new List<string>
        {
            "dmnaxbtio",
            "dmnaxbtio1g",
            "dmnaxbtio1gcommercial"
        };

        public NaxBtioDeviceFactory()
            {
            MinimumEssentialsFrameworkVersion = MinimumEssentialsVersion;
            TypeNames = _typeNames.ToList();
            }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
            {
            var props = NvxDeviceProperties.FromDeviceConfig(dc);
            return new DmNaxBtioDevice(dc.Key, dc.Name, dc);
            }
        }
    }