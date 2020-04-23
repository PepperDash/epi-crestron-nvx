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
    public class NvxDeviceFactory
    {
        public static void LoadPlugin()
        {
            DeviceFactory.AddFactoryForType("NvxDevice", BuildDevice);
            DeviceFactory.AddFactoryForType("NvxRouter", BuildRouter);
        }

        static IKeyed BuildDevice(DeviceConfig config)
        {
            var props = NvxDevicePropertiesConfig.FromDeviceConfig(config);
            var device = new NvxDeviceBuilder(config.Name, props).GetNvxDevice();

            var videoSwitcher = new NvxVideoSwitcher(config.Key, device);
            var audioSwitcher = new NvxAudioSwitcher(config.Key, device);
            var videoInputSwitcher = new NvxVideoInputHandler(config.Key, device);
            var audioInputSwitcher = new NvxAudioInputHandler(config.Key, device);
            var videoWallHelper = new NvxVideoWallHelper(config.Key, device);

            var inputs = new List<INvxHdmiInputHelper>();

            foreach (var hdmi in device.HdmiIn)
            {
                inputs.Add(new NvxHdmiInputHelper(config.Key, hdmi, device));
            }

            return new NvxDeviceEpi(config.Key, config.Name, device, props, videoSwitcher,
                audioSwitcher, videoInputSwitcher, audioInputSwitcher, videoWallHelper, inputs);
        }

        static IKeyed BuildRouter(DeviceConfig config)
        {
            var props = NvxRouterPropertiesConfig.FromDeviceConfig(config);
            return new NvxRouterEpi(config.Key, config.Name, props);
        }
    }
}