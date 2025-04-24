using System.Collections.Generic;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories;

public class NvxMockDeviceFactory : NvxBaseDeviceFactory<NvxMockDevice>
{
    public NvxMockDeviceFactory()
    {
        MinimumEssentialsFrameworkVersion = MinumumEssentialsVersion;
        TypeNames = new List<string> {"MockNvxDevice"};
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = dc.Properties.ToObject<NvxMockDeviceProperties>();
        return new NvxMockDevice(dc, props.DeviceIsTransmitter());
    }
}