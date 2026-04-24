using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories;

public class NvxDirectorFactory : EssentialsPluginDeviceFactory<NvxXioDirector>
{
    public NvxDirectorFactory()
    {
        TypeNames = new List<string>
            {
                "xiodirector",
                "xiodirector80",
                "xiodirector160",
            };
    }

    public override EssentialsDevice BuildDevice(DeviceConfig dc)
    {
        var config = JsonConvert.DeserializeObject<NvxDirectorConfig>(dc.Properties.ToString());

        DmXioDirectorBase xio = dc.Type.ToLower() switch
        {
            "xiodirector" => new DmXioDirectorEnterprise(config.Control.IpIdInt, Global.ControlSystem),
            "xiodirector80" => new DmXioDirector80(config.Control.IpIdInt, Global.ControlSystem),
            "xiodirector160" => new DmXioDirector160(config.Control.IpIdInt, Global.ControlSystem),
            _ => throw new NotSupportedException(dc.Type),
        };

        return new NvxXioDirector(dc, xio);
    }
}