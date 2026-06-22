using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using HdmiInput = NvxEpi.Features.Hdmi.Input.HdmiInput;

namespace NvxEpi.Devices;

public class NvxE20 :
    NvxBaseDevice,
    INvxE20DeviceWithHardware,
    IHdmiInput,
    IRoutingWithFeedback
{
    private IHdmiInput _hdmiInputs;

    public event RouteChangedEventHandler RouteChanged;

    public NvxE20(DeviceConfig config, Func<DmNvxBaseClass> getHardware)
        : base(config, getHardware, true)
    {
        AddPreActivationAction(AddRoutingPorts);
    }

    public override bool CustomActivate()
    {
        try
        {
            var hardware = base.Hardware as DmNvxE20 ?? throw new Exception("hardware built doesn't match");
            Hardware = hardware;

            var result = base.CustomActivate();

            _hdmiInputs = new HdmiInput(this);

            AddMcMessengers();

            if (Hardware == null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Warning, "Hardware is null", this);
                return base.CustomActivate();
            }

            Hardware.BaseEvent += (o, a) =>
            {
                var newRoute = this.HandleBaseEvent(a);

                if (newRoute == null)
                {
                    return;
                }

                RouteChanged?.Invoke(this, newRoute);
            };

            var inputPort = InputPorts.FirstOrDefault(p => p.Key == DeviceInputEnum.Hdmi1.Name);

            var outputPort = OutputPorts.FirstOrDefault(p => p.Key == SwitcherForStreamOutput.Key);

            if (inputPort == null || outputPort == null)
            {
                this.LogWarning("Unable to find input or output port for initial route. Input Port: {inputPort} Output Port: {outputPort}", inputPort, outputPort);
                return result;
            }

            var currentRoute = new RouteSwitchDescriptor(outputPort, inputPort);
            CurrentRoutes.Add(currentRoute);

            return result;
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception activating device", this);
            return false;
        }
    }

    public new DmNvxE20 Hardware { get; private set; }

    public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
    {
        get { return _hdmiInputs.HdcpCapability; }
    }

    public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
    {
        get { return _hdmiInputs.SyncDetected; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> CurrentResolution
    {
        get { return _hdmiInputs.CurrentResolution; }
    }

    public ReadOnlyDictionary<uint, IntFeedback> AudioChannels
    {
        get { return _hdmiInputs.AudioChannels; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> AudioFormat
    {
        get { return _hdmiInputs.AudioFormat; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> ColorSpace
    {
        get { return _hdmiInputs.ColorSpace; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> HdrType
    {
        get { return _hdmiInputs.HdrType; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> HdcpCapabilityString { get { return _hdmiInputs.HdcpCapabilityString; } }

    public ReadOnlyDictionary<uint, StringFeedback> HdcpSupport { get { return _hdmiInputs.HdcpSupport; } }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            if (outputSelector is not IHandleInputSwitch switcher)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "Unable to execute switch. OutputSelector is not IHandleInputSwitch {outputSelectorType}", this, outputSelector.ToString());
                return;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Switching {input} to {output} type {type}", inputSelector, outputSelector, signalType.ToString());

            if (inputSelector is null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Device is DmNvxE20. 'None' input not available", this);
                return;
            }

            switcher.HandleSwitch(inputSelector, signalType);
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Error executing switch!", this);
        }
    }

    public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        var deviceBridge = new NvxDeviceBridge(this);
        deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
    }

    private void AddRoutingPorts()
    {
        HdmiInput1Port.AddRoutingPort(this);
        SwitcherForStreamOutput.AddRoutingPort(this);
        AnalogAudioInput.AddRoutingPort(this);
        SecondaryAudioInput.AddRoutingPort(this);
        SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
    }
}