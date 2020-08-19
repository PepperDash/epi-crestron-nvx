using System.Collections.Generic;
using System.Linq;
using NvxEpi.DynRouting.Builder;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.DynRouting.Factories
{
    public class DynNvxFactory : EssentialsPluginDeviceFactory<DynNvx>
    {
        private const string _minumumEssentialsVersion = "1.6.1";
        private static readonly IEnumerable<string> _typeNames;

        static DynNvxFactory()
        {
            _typeNames = new List<string>() {"dynnvx"};
        }

        public DynNvxFactory()
        {
            MinimumEssentialsFrameworkVersion = _minumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            return new DynNvxDeviceBuilder(dc).Build();
        }
    }
}