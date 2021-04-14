using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Aggregates;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class Nvx36XDeviceFactory : NvxBaseDeviceFactory<Nvx36X>
    {
        private static readonly List<string> _typeNames;

        static Nvx36XDeviceFactory()
        {
            _typeNames = typeof(DmNvxBaseClass)
                .GetCType()
                .Assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(DmNvx36x).GetCType()) && !x.IsAbstract)
                .Select(x => x.Name)
                .ToList();
        }

        public Nvx36XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var device = BuildDeviceFromConfig(dc);
            var hardware = device as DmNvx36x;
            if (hardware == null)
                throw new ArgumentException("type");

            return new Nvx36X(dc, hardware);
        }
    }
}