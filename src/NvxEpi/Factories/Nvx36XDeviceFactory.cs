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
    public class Nvx36XDeviceFactory : NvxBaseDeviceFactory<Nvx36X>
    {
        private static IEnumerable<string> _typeNames;

        public Nvx36XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

            if (_typeNames == null)
            {
                _typeNames = new List<string>
                {
                    "dmnvx360",
                    "dmnvx360c",
                    "dmnvx363",
                };
            }

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