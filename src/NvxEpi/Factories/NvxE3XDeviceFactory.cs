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
    public class NvxE3XDeviceFactory : NvxBaseDeviceFactory<NvxE3X>
    {
        private static readonly List<string> _typeNames;

        static NvxE3XDeviceFactory()
        {
            _typeNames = _types
                .Values
                .Where(x => x.IsSubclassOf(typeof(DmNvxE3x).GetCType()))
                .Select(x => x.Name)
                .ToList();
        }

        public NvxE3XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;
            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var device = BuildDeviceFromConfig(dc);
            var hardware = device as DmNvxE3x;
            if (hardware == null)
                throw new ArgumentException("type");

            return new NvxE3X(dc, hardware);
        }
    }
}