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
        private static List<string> _typeNames;

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
            var device = BuildDeviceFromConfig(dc);
            var hardware = device as DmNvxD3x;
            if (hardware == null)
                throw new ArgumentException("type");

            return new NvxD3X(dc, hardware);
        }
    }
}