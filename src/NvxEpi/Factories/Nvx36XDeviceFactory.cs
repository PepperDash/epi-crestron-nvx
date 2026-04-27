using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Serilog.Events;

namespace NvxEpi.Factories;

public class Nvx36XDeviceFactory : NvxBaseDeviceFactory<Nvx36X>
{
    private static IEnumerable<string> _typeNames;

    public Nvx36XDeviceFactory()
    {
        MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

        _typeNames ??= new List<string>
            {
                "dmnvx360",
                "dmnvx360c",
                "dmnvx363",
                "dmnvx363c",
                "dmnvxe760",
                "dmnvxe760c",
            };

        TypeNames = _typeNames.ToList();
    }

    private static readonly string[] _usbUnsupportedTypes = { "dmnvxe760", "dmnvxe760c" };

    private static void WarnIfUsbConfiguredOnUnsupportedDevice(DeviceConfig dc, NvxDeviceProperties props)
    {
        if (props.Usb == null)
            return;

        var isUnsupported = Array.Exists(_usbUnsupportedTypes, t => t.Equals(dc.Type, StringComparison.OrdinalIgnoreCase));

        if (isUnsupported)
            Debug.LogMessage(LogEventLevel.Warning, "Device '{key}' of type '{type}' does not support USB, but USB is configured in config. USB configuration will be ignored.", dc.Key, dc.Type);
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = NvxDeviceProperties.FromDeviceConfig(dc);
        WarnIfUsbConfiguredOnUnsupportedDevice(dc, props);
        var deviceBuild = GetDeviceBuildAction(dc.Type, props);
        return new Nvx36X(dc, deviceBuild, props.DeviceIsTransmitter());
    }
}