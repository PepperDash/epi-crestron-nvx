using System.Collections.Generic;
using System.Linq;
using NvxEpi.Application.Builder;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Application.Factories
{
    public class NvxApplicationFactory : EssentialsPluginDeviceFactory<NvxApplication>
    {
        private const string _minumumEssentialsVersion = "1.8.0";
        private static readonly IEnumerable<string> _typeNames;

        static NvxApplicationFactory()
        {
            _typeNames = new List<string>() {"dynnvx", "nvxapplication", "nvxapp"};
        }

        public NvxApplicationFactory()
        {
            MinimumEssentialsFrameworkVersion = _minumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            return new NvxApplicationApplicationBuilder(dc).Build();
        }
    }
}