using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Devices;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class NvxD3XDeviceFactory : NvxBaseDeviceFactory<NvxD3X>
    {
        private static readonly List<string> _typeNames;

        static NvxD3XDeviceFactory()
        {
            _typeNames = typeof(DmNvxBaseClass)
                .GetCType()
                .Assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(DmNvxD3x).GetCType()) && !x.IsAbstract)
                .Select(x => x.Name)
                .ToList();
        }

        public NvxD3XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var device = BuildDeviceFromConfig(dc);
            var hardware = device as DmNvxD3x;
            if (hardware == null)
                throw new ArgumentException("type");

            return new NvxD3X(dc, hardware);
        }
    }
}