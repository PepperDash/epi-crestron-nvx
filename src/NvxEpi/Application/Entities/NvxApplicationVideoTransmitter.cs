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
using NvxEpi.Abstractions.UsbcInput;

namespace NvxEpi.Application.Entities;

public class NvxApplicationVideoTransmitter : EssentialsDevice, IOnline
{
    public int DeviceId { get; private set; }
    public BoolFeedback VideoSyncDetected { get; private set; }
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
    private bool _useUsbInput1;
    private bool _useUsbInput2;

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
                    Debug.LogMessage(0, this, "Caught an exception:{0}", ex);
                }
            });
    }

    private void LinkRoutingInputPort(string routingPortKey)
    {
        if (string.IsNullOrEmpty(routingPortKey) || Device is DmNvxE3x)
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.NoSwitch.Name] ?? throw new NullReferenceException(DeviceInputEnum.NoSwitch.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Hdmi1.Name, StringComparison.OrdinalIgnoreCase))
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.Hdmi1.Name] ?? throw new NullReferenceException(DeviceInputEnum.Hdmi1.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Hdmi2.Name, StringComparison.OrdinalIgnoreCase))
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.Hdmi2.Name] ?? throw new NullReferenceException(DeviceInputEnum.Hdmi2.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Usbc1.Name, StringComparison.OrdinalIgnoreCase))
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.Usbc1.Name] ?? throw new NullReferenceException(DeviceInputEnum.Usbc1.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Usbc2.Name, StringComparison.OrdinalIgnoreCase))
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.Usbc2.Name] ?? throw new NullReferenceException(DeviceInputEnum.Usbc2.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Automatic.Name, StringComparison.OrdinalIgnoreCase))
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.Automatic.Name] ?? throw new NullReferenceException(DeviceInputEnum.Automatic.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else
        {
            Debug.LogWarning(this, "----- {0} is not a valid routing port key, available ports are:", routingPortKey);
            Device
                .InputPorts
                .ToList()
                .ForEach(x => Debug.LogWarning(this, "----- " + x.Key));

            throw new NotSupportedException(routingPortKey);
        }
    }

    private void LinkInputValues(string routingPortKey)
    {
        VideoSyncDetected = new BoolFeedback(() => false);
        HdcpState = new IntFeedback(() => 0);
        HdcpCapability = new IntFeedback(() => 99);
        InputResolution = new StringFeedback(() => string.Empty);

        if (string.IsNullOrEmpty(routingPortKey))
        {
            if (Device is not IHdmiInput videoInput)
                return;

            VideoSyncDetected = videoInput.SyncDetected[1];
            HdcpState = videoInput.HdcpCapability[1];
            InputResolution = videoInput.CurrentResolution[1];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Hdmi1.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not IHdmiInput videoInput)
                return;

            VideoSyncDetected = videoInput.SyncDetected[1];
            HdcpState = videoInput.HdcpCapability[1];
            InputResolution = videoInput.CurrentResolution[1];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Hdmi2.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not IHdmiInput videoInput)
                return;

            _useHdmiInput2 = true;
            VideoSyncDetected = videoInput.SyncDetected[2];
            HdcpState = videoInput.HdcpCapability[2];
            InputResolution = videoInput.CurrentResolution[2];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Usbc1.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not INvxUsbcInput videoInput)
                return;

            _useUsbInput1 = true;
            VideoSyncDetected = videoInput.SyncDetected[3];
            HdcpState = videoInput.HdcpCapability[3];
            InputResolution = videoInput.CurrentResolution[3];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Usbc2.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not INvxUsbcInput videoInput)
                return;

            _useUsbInput2 = true;
            VideoSyncDetected = videoInput.SyncDetected[4];
            HdcpState = videoInput.HdcpCapability[4];
            InputResolution = videoInput.CurrentResolution[4];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Automatic.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not IHdmiInput hdmiInput)
                return;

            if (Device is not ICurrentVideoInput hdmiSwitcher)
                return;

            VideoSyncDetected = hdmiInput.SyncDetected[1];
            HdcpState = hdmiInput.HdcpCapability[1];
            InputResolution = hdmiInput.CurrentResolution[1];
        }
        else
            throw new NotSupportedException(routingPortKey);

        VideoSyncDetected.FireUpdate();
        HdcpCapability.FireUpdate();
        InputResolution.FireUpdate();
    }

    public BoolFeedback IsOnline
    {
        get { return Device.IsOnline; }
    }

    public void SetHdcpState(ushort state)
    {
        if (Device is IHdmiInput videoInput)
        {
            if (_useHdmiInput2)
                videoInput.SetHdmi2HdcpCapability(state);
            else
                videoInput.SetHdmi1HdcpCapability(state);
        }
        else if (Device is INvxUsbcInput videoInputWithUsbc)
        {
            if (_useHdmiInput2 && Device is IHdmiInput hdmiCapableUsbc)
                hdmiCapableUsbc.SetHdmi2HdcpCapability(state);
            else if (_useUsbInput1)
                videoInputWithUsbc.SetUsbc1HdcpCapability(state);
            else if (_useUsbInput2)
                videoInputWithUsbc.SetUsbc2HdcpCapability(state);
        }
    }
}