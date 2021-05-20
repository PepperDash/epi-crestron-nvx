using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class NvxD3XDeviceFactory : NvxBaseDeviceFactory<NvxD3X>
    {
        private static IEnumerable<string> _typeNames;

        public NvxD3XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;
            
            if (_typeNames == null)
            {
                _typeNames = new List<string>
                {
                    "dmnvxd30",
                    "dmnvxd30c",
                };
            }

            TypeNames = _typeNames.ToList();
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(dc);
            var deviceBuild = GetDeviceBuildAction(dc.Type, props);
            return new NvxD3X(dc, deviceBuild);
        }
    }
}