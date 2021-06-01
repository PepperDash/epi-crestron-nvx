using System;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Application.Config;
using NvxEpi.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application.Entities
{
    public class NvxApplicationAudioTransmitter : EssentialsDevice
    {
        public int DeviceId { get; private set; }
        public StringFeedback AudioName { get; private set; }

        private readonly DummyRoutingInputsDevice _source;

        public IRoutingSource Source
        {
            get { return _source; }
        }

        public INvxDevice Device { get; private set; }

        public NvxApplicationAudioTransmitter(string key, NvxApplicationDeviceAudioConfig config, int deviceId)
            : base(key)
        {
            DeviceId = deviceId;
            _source = new DummyRoutingInputsDevice(config.DeviceKey + "--audioSource");

            AddPostActivationAction(() =>
                {
                    Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as INvxDevice;
                    if (Device == null)
                        throw new NullReferenceException("device");
                });

            AddPostActivationAction(() =>
                {
                    Name = Device.Name;
                    AudioName =
                        new StringFeedback(() => string.IsNullOrEmpty(config.AudioName) ? Device.Name : config.AudioName);
                    AudioName.FireUpdate();
                });

            AddPostActivationAction(() => LinkRoutingInputPort(config.NvxRoutingPort));
        }

        private void LinkRoutingInputPort(string routingPortKey)
        {
            if (string.IsNullOrEmpty(routingPortKey))
            {
                var routingPort = Device.InputPorts[DeviceInputEnum.NoSwitch.Name];
                if (routingPort == null)
                    throw new NullReferenceException(DeviceInputEnum.NoSwitch.Name);

                TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
            }
            else if (routingPortKey.Equals(DeviceInputEnum.Hdmi1.Name, StringComparison.OrdinalIgnoreCase))
            {
                var routingPort = Device.InputPorts[DeviceInputEnum.Hdmi1.Name];
                if (routingPort == null)
                    throw new NullReferenceException(DeviceInputEnum.Hdmi1.Name);

                TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
            }
            else if (routingPortKey.Equals(DeviceInputEnum.Hdmi2.Name, StringComparison.OrdinalIgnoreCase))
            {
                var routingPort = Device.InputPorts[DeviceInputEnum.Hdmi2.Name];
                if (routingPort == null)
                    throw new NullReferenceException(DeviceInputEnum.Hdmi2.Name);

                TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
            }
            else if (routingPortKey.Equals(DeviceInputEnum.AnalogAudio.Name, StringComparison.OrdinalIgnoreCase))
            {
                var routingPort = Device.InputPorts[DeviceInputEnum.AnalogAudio.Name];
                if (routingPort == null)
                    throw new NullReferenceException(DeviceInputEnum.AnalogAudio.Name);

                TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
            }
            else if (routingPortKey.Equals(DeviceInputEnum.Automatic.Name, StringComparison.OrdinalIgnoreCase))
            {
                var routingPort = Device.InputPorts[DeviceInputEnum.Automatic.Name];
                if (routingPort == null)
                    throw new NullReferenceException(DeviceInputEnum.Automatic.Name);

                TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
            }
            else
            {
                Debug.Console(1, this, "----- {0} is not a valid routing port key, available ports are:", routingPortKey);
                Device
                    .InputPorts
                    .ToList()
                    .ForEach(x => Debug.Console(1, this, "----- " + x.Key));

                throw new NotSupportedException(routingPortKey);
            }
        }
    }
}