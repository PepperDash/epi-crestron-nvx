using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Application.Config;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Application.Entities;

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
                NameFeedback = new StringFeedback("name", () => Device.Name);
                VideoName =
                    new StringFeedback("videoName", () => string.IsNullOrEmpty(config.VideoName) ? Device.Name : config.VideoName);
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
                    this.LogError("Caught an exception: {message}", ex.Message);
                    this.LogDebug(ex, "Stack Trace: ");
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
        else if (routingPortKey.Equals(DeviceInputEnum.Automatic.Name, StringComparison.OrdinalIgnoreCase))
        {
            var routingPort = Device.InputPorts[DeviceInputEnum.Automatic.Name] ?? throw new NullReferenceException(DeviceInputEnum.Automatic.Name);
            TieLineCollection.Default.Add(new TieLine(_source.AudioVideoOutputPort, routingPort));
        }
        else
        {
            this.LogDebug("----- {key} is not a valid routing port key, available ports are:", routingPortKey);
            Device
                .InputPorts
                .ToList()
                .ForEach(x => this.LogDebug("----- " + x.Key));

            throw new NotSupportedException(routingPortKey);
        }
    }

    private void LinkInputValues(string routingPortKey)
    {
        HdmiSyncDetected = new BoolFeedback("hdmiSyncDetected", () => false);
        HdcpState = new IntFeedback("hdcpState", () => 0);
        HdcpCapability = new IntFeedback("hdcpCapability", () => 99);
        InputResolution = new StringFeedback("inputResolution", () => string.Empty);

        if (string.IsNullOrEmpty(routingPortKey))
        {
            if (Device is not IHdmiInput hdmiInput)
                return;

            HdmiSyncDetected = hdmiInput.SyncDetected[1];
            HdcpState = hdmiInput.HdcpCapability[1];
            InputResolution = hdmiInput.CurrentResolution[1];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Hdmi1.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not IHdmiInput hdmiInput)
                return;

            HdmiSyncDetected = hdmiInput.SyncDetected[1];
            HdcpState = hdmiInput.HdcpCapability[1];
            InputResolution = hdmiInput.CurrentResolution[1];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Hdmi2.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not IHdmiInput hdmiInput)
                return;

            _useHdmiInput2 = true;
            HdmiSyncDetected = hdmiInput.SyncDetected[2];
            HdcpState = hdmiInput.HdcpCapability[2];
            InputResolution = hdmiInput.CurrentResolution[2];
        }
        else if (routingPortKey.Equals(DeviceInputEnum.Automatic.Name, StringComparison.OrdinalIgnoreCase))
        {
            if (Device is not IHdmiInput hdmiInput)
                return;

            if (Device is not ICurrentVideoInput hdmiSwitcher)
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
        if (Device is not IHdmiInput hdmiInput)
            return;

        if (_useHdmiInput2)
            hdmiInput.SetHdmi2HdcpCapability(state);
        else
            hdmiInput.SetHdmi1HdcpCapability(state);
    }
}