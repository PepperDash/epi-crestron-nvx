using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Entities.Aggregates;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Factories
{
    public class Nvx35XDeviceFactory : EssentialsPluginDeviceFactory<Nvx35x>
    {
        private const string _minumumEssentialsVersion = "1.5.8";
        private static readonly IEnumerable<string> _typeNames;

        static Nvx35XDeviceFactory()
        {
            _typeNames = typeof (DmNvxBaseClass)
                .GetCType()
                .Assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof (DmNvx35x).GetCType()))
                .Select(x => x.Name);
        }

        public Nvx35XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = _minumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var nvxDevice = NvxDeviceBuilder
                .Create(dc)
                .Build();

            return new Nvx35x(nvxDevice);
        }
    }
}