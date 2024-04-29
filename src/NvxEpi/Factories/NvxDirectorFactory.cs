using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Factories
{
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

            DmXioDirectorBase xio;
            switch (dc.Type.ToLower())
            {
                case "xiodirector":
                    xio = new DmXioDirectorEnterprise(config.Control.IpIdInt, Global.ControlSystem);
                    break;
                case "xiodirector80":
                    xio = new DmXioDirector80(config.Control.IpIdInt, Global.ControlSystem);
                    break;
                case "xiodirector160":
                    xio = new DmXioDirector160(config.Control.IpIdInt, Global.ControlSystem);
                    break;
                default:
                    throw new NotSupportedException(dc.Type);
            }

            xio.RegisterWithLogging(dc.Key);
            return new NvxXioDirector(dc.Key, dc.Name, xio);
        }
    }
}