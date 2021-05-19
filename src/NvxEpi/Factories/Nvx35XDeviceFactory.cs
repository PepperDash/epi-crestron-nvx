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
    public class Nvx35XDeviceFactory : NvxBaseDeviceFactory<Nvx35X>
    {
        private static IEnumerable<string> _typeNames;

        public Nvx35XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

            if (_typeNames == null)
            {
                _typeNames = new List<string>
                {
                    "dmnvx350",
                    "dmnvx350c",
                    "dmnvx351",
                    "dmnvx351c",
                    "dmnvx352",
                    "dmnvx352c",
                };
            }

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