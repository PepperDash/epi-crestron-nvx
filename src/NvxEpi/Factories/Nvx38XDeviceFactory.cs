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
                "dmnvx385c"
            };

        TypeNames = _typeNames.ToList();
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = NvxDeviceProperties.FromDeviceConfig(dc);
        var deviceBuild = GetDeviceBuildAction(dc.Type, props);
        return new Nvx38X(dc, deviceBuild, props.DeviceIsTransmitter());
    }
}