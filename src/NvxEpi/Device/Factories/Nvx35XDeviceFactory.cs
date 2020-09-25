using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Entities.Aggregates;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Factories
{
    public class Nvx35XDeviceFactory : NvxBaseDeviceFactory<Nvx35X>
    {
        private const string _minumumEssentialsVersion = "1.6.4";
        private static readonly List<string> _typeNames;

        static Nvx35XDeviceFactory()
        {
            _typeNames = typeof (DmNvxBaseClass)
                .GetCType()
                .Assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof (DmNvx35x).GetCType()))
                .Select(x => x.Name)
                .ToList();
        }

        public Nvx35XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = _minumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var device = BuildDeviceFromConfig(dc);
            var hardware = device as DmNvx35x;
            if (hardware == null)
                throw new ArgumentException("type");

            return new Nvx35X(dc, hardware);
        }
    }
}