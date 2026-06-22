using System.Collections.Generic;
using System.Linq;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories;

public class NvxE20DeviceFactory : NvxBaseDeviceFactory<NvxE20>
{
    private static List<string> _typeNames;

    public NvxE20DeviceFactory()
    {
        MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;

        _typeNames ??= new List<string>
            {
                "dmnvxe20",
                "dmnvxe202g"
            };

        TypeNames = _typeNames.ToList();
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = NvxDeviceProperties.FromDeviceConfig(dc);
        var deviceBuild = GetDeviceBuildAction(dc.Type, props);
        return new NvxE20(dc, deviceBuild);
    }
}
