using System.Collections.Generic;
using System.Linq;
using NvxEpi.Application.Builder;
using NvxEpi.Devices;
using NvxEpi.Factories;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Application.Factories
{
    public class NvxApplicationFactory : EssentialsPluginDeviceFactory<NvxApplication>
    {
        private static readonly IEnumerable<string> _typeNames;

        static NvxApplicationFactory()
        {
            _typeNames = new List<string>() {"dynnvx", "nvxapplication", "nvxapp"};
        }

        public NvxApplicationFactory()
        {
            MinimumEssentialsFrameworkVersion = NvxBaseDeviceFactory<NvxBaseDevice>.MinumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            return new NvxApplicationApplicationBuilder(dc).Build();
        }
    }
}