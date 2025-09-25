using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories;

public class Nvx38XDeviceFactory : NvxBaseDeviceFactory<Nvx38X>
{
    private static IEnumerable<string> _typeNames;

    public Nvx38XDeviceFactory()
    {
        MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

        _typeNames ??= new List<string>
            {
                "dmnvx384",
                "dmnvx384c",
                "dmnvx385",
                "dmnvx385c",
            };

        TypeNames = _typeNames.ToList();
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = NvxDeviceProperties.FromDeviceConfig(dc);
        var deviceBuild = GetDeviceBuildAction(dc.Type, props);
        
        // Check if this device has multiview configuration
        var multiviewConfig = dc.Properties?.ToObject<Nvx38xMultiviewConfig>();
        
        // If multiview config is provided, use the multiview constructor
        if (multiviewConfig?.Screens != null && multiviewConfig.Screens.Any())
        {
            return new Nvx38X(dc, deviceBuild, props.DeviceIsTransmitter(), multiviewConfig);
        }
        
        // Otherwise, use the basic constructor
        return new Nvx38X(dc, deviceBuild, props.DeviceIsTransmitter());
    }
}