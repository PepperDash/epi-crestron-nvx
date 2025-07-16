using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories;

public class NvxE76XDeviceFactory : NvxBaseDeviceFactory<NvxE76x>
{
    private static IEnumerable<string> _typeNames;

    public NvxE76XDeviceFactory()
    {
        MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

        _typeNames ??= new List<string>
            {
                "dmnvxe760",
                "dmnvxe760c",
            };

        TypeNames = _typeNames.ToList();
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = NvxDeviceProperties.FromDeviceConfig(dc);
        var deviceBuild = GetDeviceBuildAction(dc.Type, props);
        return new NvxE76x(dc, deviceBuild, props.DeviceIsTransmitter());
    }
}