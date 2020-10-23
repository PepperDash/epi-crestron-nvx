using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;

using NvxEpi.DeviceHelpers;
using NvxEpi.Interfaces;
using NvxEpi.Routing;

using PepperDash.Core;
using PepperDash.Essentials.Bridges;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

using Newtonsoft.Json;

namespace NvxEpi.Factory
{
    public class NvxDeviceFactory : IKeyed
    {
        private readonly DeviceConfig _config;

        public static void LoadPlugin()
        {
            DeviceFactory.AddFactoryForType("NvxDevice", config => new NvxDeviceFactory(config).BuildDevice());
            DeviceFactory.AddFactoryForType("NvxRouter", BuildRouter);
        }

        public NvxDeviceFactory(DeviceConfig config)
        {
            _config = config;
        }

        public IKeyed BuildDevice()
        {
            Debug.Console(2, this, "Building a new NVX device {0}... Wish me luck!", _config.Key);

            var props = NvxDevicePropertiesConfig.FromDeviceConfig(_config);
            var device = new NvxDeviceBuilder(_config.Name, props).GetNvxDevice();

            var videoSwitcher = new NvxVideoSwitcher(_config.Key, device);
            var audioSwitcher = new NvxAudioSwitcher(_config.Key, device);
            var videoInputSwitcher = new NvxVideoInputHandler(_config.Key, device);
            var audioInputSwitcher = new NvxAudioInputHandler(_config.Key, device);
            var videoWallHelper = new NvxVideoWallHelper(_config.Key, device);

            var inputs = new List<INvxHdmiInputHelper>();

            if (device.HdmiIn == null)
                return new NvxDeviceEpi(_config.Key, _config.Name, device, props, videoSwitcher,
                    audioSwitcher, videoInputSwitcher, audioInputSwitcher, videoWallHelper, inputs);

            for (uint x = 1; x <= device.HdmiIn.Count; x++)
            {
                if (device.HdmiIn[x] == null)
                    continue;

                inputs.Add(new NvxHdmiInputHelper(_config.Key + "-HdmiIn-" + x, device.HdmiIn[x], device));
            }

            return new NvxDeviceEpi(_config.Key, _config.Name, device, props, videoSwitcher,
                audioSwitcher, videoInputSwitcher, audioInputSwitcher, videoWallHelper, inputs);
        }

        static IKeyed BuildRouter(DeviceConfig config)
        {
            var props = NvxRouterPropertiesConfig.FromDeviceConfig(config);
            return new NvxRouterEpi(config.Key, config.Name, props);
        }

        #region IKeyed Members

        public string Key
        {
            get { return "NvxFactory"; }
        }

        #endregion
    }
}