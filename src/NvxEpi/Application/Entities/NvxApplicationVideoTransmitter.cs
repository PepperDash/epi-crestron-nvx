using System;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Application.Config;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Application.Entities
{
    public class NvxApplicationVideoTransmitter : EssentialsDevice, IOnline
    {
        public int DeviceId { get; private set; }
        public BoolFeedback HdmiSyncDetected { get; private set; }
        public IntFeedback HdcpState { get; private set; }
        public IntFeedback HdcpCapability { get; private set; }
        public StringFeedback InputResolution { get; private set; }
        public StringFeedback NameFeedback { get; private set; }
        public StringFeedback VideoName { get; private set; }

        private readonly DummyRoutingInputsDevice _source;

        public IRoutingSource Source
        {
            get { return _source; }
        }

        public INvxDevice Device { get; private set; }

        private bool _useHdmiInput2;

        public NvxApplicationVideoTransmitter(string key, NvxApplicationDeviceVideoConfig config, int deviceId)
            : base(key)
        {
            DeviceId = deviceId;
            _source = new DummyRoutingInputsDevice(config.DeviceKey + "--videoSource");

            AddPostActivationAction(() =>
                {
                    Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as INvxDevice;
                    if (Device == null)
                        throw new NullReferenceException("device");
                });

            AddPostActivationAction(() =>
                {
                    Name = Device.Name;
                    NameFeedback = new StringFeedback(() => Device.Name);
                    VideoName =
                        new StringFeedback(() => string.IsNullOrEmpty(config.VideoName) ? Device.Name : config.VideoName);
                    NameFeedback.FireUpdate();
                    VideoName.FireUpdate();
                });

            AddPostActivationAction(() =>
                {
                    try
                    {

                        LinkRoutingInputPort(config.NvxRoutingPort);
                        LinkInputValues(config.NvxRoutingPort);
                    }
                    catch (Exception ex)
                    {
                        Debug.Console(0, this, "Caught an exception:{0}", ex);
                    }
                });
        }

        private void LinkRoutingInputPort(string routingPortKey)
        {
            if (string.IsNullOrEmpty(routingPortKey) || Device is DmNvxE3x)
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

        private void LinkInputValues(string routingPortKey)
        {
            HdmiSyncDetected = new BoolFeedback(() => false);
            HdcpState = new IntFeedback(() => 0);
            HdcpCapability = new IntFeedback(() => 99);
            InputResolution = new StringFeedback(() => string.Empty);

            if (string.IsNullOrEmpty(routingPortKey))
            {
                var hdmiInput = Device as IHdmiInput;
                if (hdmiInput == null)
                    return;

                HdmiSyncDetected = hdmiInput.SyncDetected[1];
                HdcpState = hdmiInput.HdcpCapability[1];
                InputResolution = hdmiInput.CurrentResolution[1];
            }
            else if (routingPortKey.Equals(DeviceInputEnum.Hdmi1.Name, StringComparison.OrdinalIgnoreCase))
            {
                var hdmiInput = Device as IHdmiInput;
                if (hdmiInput == null)
                    return;

                HdmiSyncDetected = hdmiInput.SyncDetected[1];
                HdcpState = hdmiInput.HdcpCapability[1];
                InputResolution = hdmiInput.CurrentResolution[1];
            }
            else if (routingPortKey.Equals(DeviceInputEnum.Hdmi2.Name, StringComparison.OrdinalIgnoreCase))
            {
                var hdmiInput = Device as IHdmiInput;
                if (hdmiInput == null)
                    return;

                _useHdmiInput2 = true;
                HdmiSyncDetected = hdmiInput.SyncDetected[2];
                HdcpState = hdmiInput.HdcpCapability[2];
                InputResolution = hdmiInput.CurrentResolution[2];
            }
            else if (routingPortKey.Equals(DeviceInputEnum.Automatic.Name, StringComparison.OrdinalIgnoreCase))
            {
                var hdmiInput = Device as IHdmiInput;
                if (hdmiInput == null)
                    return;

                var hdmiSwitcher = Device as ICurrentVideoInput;
                if (hdmiSwitcher == null)
                    return;

                HdmiSyncDetected = hdmiInput.SyncDetected[1];
                HdcpState = hdmiInput.HdcpCapability[1];
                InputResolution = hdmiInput.CurrentResolution[1];
            }
            else
                throw new NotSupportedException(routingPortKey);

            HdmiSyncDetected.FireUpdate();
            HdcpCapability.FireUpdate();
            InputResolution.FireUpdate();
        }

        public BoolFeedback IsOnline
        {
            get { return Device.IsOnline; }
        }

        public void SetHdcpState(ushort state)
        {
            var hdmiInput = Device as IHdmiInput;
            if (hdmiInput == null)
                return;

            if (_useHdmiInput2)
                hdmiInput.SetHdmi2HdcpCapability(state);
            else
                hdmiInput.SetHdmi1HdcpCapability(state);
        }
    }
}