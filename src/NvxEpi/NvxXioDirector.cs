using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using System;
using System.Collections.Generic;

namespace NvxEpi
{
    internal class NvxXioDirector : EssentialsDevice
    {
        public class NvxDirectorConfig
        {
            public ControlPropertiesConfig? Control { get; set; }
            public List<NvxDirectorDomainConfig> Domains { get; set; } = new List<NvxDirectorDomainConfig>();
        }

        public class NvxDirectorDomainConfig
        {
            public uint Id { get; set; }
            public List<DeviceConfig> Transmitters { get; set; } = new List<DeviceConfig>();
            public List<DeviceConfig> Receivers { get; set; } = new List<DeviceConfig>();
        }

        private readonly DmXioDirectorBase xio;

        public NvxXioDirector(DeviceConfig config) : base(config.Key, config.Name)
        {
            var props = config.Properties.ToObject<NvxDirectorConfig>() ?? throw new ArgumentException("Invalid configuration");
            var controlProps = props.Control ?? throw new ArgumentException("Control properties are required");

            xio = config.Type.ToLowerInvariant() switch
            {
                "xiodirector" => new DmXioDirectorEnterprise(controlProps.IpIdInt, Global.ControlSystem),
                "xiodirector80" => new DmXioDirector80(controlProps.IpIdInt, Global.ControlSystem),
                "xiodirector160" => new DmXioDirector160(controlProps.IpIdInt, Global.ControlSystem),
                _ => throw new NotSupportedException(config.Type),
            };

            foreach (var domainConfig in props.Domains)
            {
                if (xio.Domain.Contains(domainConfig.Id))
                {
                    this.LogError("Domain {id} already exists, skipping add.", domainConfig.Id);
                    continue;
                }

                var domain = new DmXioDirectorBase.DmXioDomain(domainConfig.Id, xio);
                this.LogDebug("Adding domain: {id}", domain.Id);

                foreach (var deviceConfig in domainConfig.Transmitters)
                {
                    var deviceProps = deviceConfig.Properties.ToObject<NvxDeviceProperties>() ?? throw new ArgumentException($"Invalid transmitter config for device {deviceConfig.Key}");

                    // Set the mode to "tx" for transmitter devices
                    deviceProps.Mode = "tx";

                    var device = NvxDeviceFactory.BuildDevice(deviceConfig.Key, deviceConfig.Type, deviceProps, domain);
                    if (device != null)
                    {
                        this.LogDebug("Adding transmitter: {key} to domain {domainId}", device.Key, domain.Id);
                        DeviceManager.AddDevice(device);
                    }
                    else
                    {
                        this.LogError("Failed to create transmitter device for config: {key}", deviceConfig.Key);
                    }
                }

                foreach (var deviceConfig in domainConfig.Receivers)
                {
                    var deviceProps = deviceConfig.Properties.ToObject<NvxDeviceProperties>() ?? throw new ArgumentException($"Invalid receiver config for device {deviceConfig.Key}");

                    // Set the mode to "rx" for receiver devices
                    deviceProps.Mode = "rx";

                    var device = NvxDeviceFactory.BuildDevice(deviceConfig.Key, deviceConfig.Type, deviceProps, domain);
                    if (device != null)
                    {
                        this.LogDebug("Adding receiver: {key} to domain {domainId}", device.Key, domain.Id);
                        DeviceManager.AddDevice(device);
                    }
                    else
                    {
                        this.LogError("Failed to create receiver device for config: {key}", deviceConfig.Key);
                    }
                }
            }
        }

        public override void Initialize() => 
            CrestronInvoke.BeginInvoke(_ =>
                {
                    if (xio.Registerable)
                    {
                        var result = xio.RegisterWithLogging(Key);

                        if (result != eDeviceRegistrationUnRegistrationResponse.Success)
                        {
                            this.LogError("Failed to register XIO Director: {result} {reason}", result, xio.RegistrationFailureReason);
                        }
                    }
                });
    }
}
