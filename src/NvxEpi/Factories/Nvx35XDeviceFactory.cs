﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Aggregates;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
    public class Nvx35XDeviceFactory : NvxBaseDeviceFactory<Nvx35X>
    {
        private static readonly List<string> _typeNames;

        static Nvx35XDeviceFactory()
        {
            _typeNames = _types
                .Values
                .Where(x => x.IsSubclassOf(typeof (DmNvx35x).GetCType()))
                .Select(x => x.Name)
                .ToList();
        }

        public Nvx35XDeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;
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