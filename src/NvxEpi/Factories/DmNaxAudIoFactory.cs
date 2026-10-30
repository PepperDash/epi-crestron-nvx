using System.Collections.Generic;
using Crestron.SimplSharpPro.AudioDistribution;
using NvxEpi.Devices;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories;

public class DmNaxAudIoFactory : EssentialsPluginDeviceFactory<DmNaxAudIo>
{
    public DmNaxAudIoFactory()
    {
        MinimumEssentialsFrameworkVersion = NvxBaseDeviceFactory<DmNaxAudIo>.MinumumEssentialsVersion;
        TypeNames = new List<string> { "dmnaxaudio" };
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var props = dc.Properties.ToObject<DmNaxAudIoProperties>();
        var hardware = new DmNaxAudIoCommercial(props.Control.IpIdInt, Global.ControlSystem);

        Debug.LogInformation("Building DM-NAX-AUD-IO {key} at IP ID {ipid:X2}", dc.Key, props.Control.IpIdInt);
        return new DmNaxAudIo(dc, props, hardware);
    }
}
