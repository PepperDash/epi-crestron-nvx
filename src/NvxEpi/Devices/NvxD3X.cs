using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Extensions;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices;

public class NvxD3X : 
    NvxBaseDevice, 
    INvxD3XDeviceWithHardware, 
    IComPorts, 
    IIROutputPorts,
    IHdmiOutput,
    IRoutingWithFeedback
{
    private IHdmiOutput _hdmiOutput;
    private readonly IUsbStream _usbStream;

    public event RouteChangedEventHandler RouteChanged;

    public NvxD3X(DeviceConfig config, Func<DmNvxBaseClass> getHardware)
        : base(config, getHardware, false)
    {
        AddPreActivationAction(AddRoutingPorts);
    }

    public override bool CustomActivate()
    {
        var hardware = base.Hardware as DmNvxD3x ?? throw new Exception("hardware built doesn't match");
        Hardware = hardware;

        var result = base.CustomActivate();

        _hdmiOutput = new HdmiOutput(this);

        Hardware.BaseEvent += (o, a) => {
            var newRoute = this.HandleBaseEvent(a);

            if (newRoute == null)
            {
                return;
            }

            RouteChanged?.Invoke(this, newRoute);
        };

        return result;
    }

    public CrestronCollection<ComPort> ComPorts
    {
        get { return Hardware.ComPorts; }
    }

    public BoolFeedback DisabledByHdcp
    {
        get { return _hdmiOutput.DisabledByHdcp; }
    }

    public new DmNvxD3x Hardware { get; private set; }

    public IntFeedback HorizontalResolution
    {
        get { return _hdmiOutput.HorizontalResolution; }
    }

    public StringFeedback EdidManufacturer
    {
        get { return _hdmiOutput.EdidManufacturer; }
    }

    public StringFeedback OutputResolution
    {
        get { return _hdmiOutput.OutputResolution; }
    }

    public CrestronCollection<IROutputPort> IROutputPorts
    {
        get { return Hardware.IROutputPorts; }
    }

    public bool IsRemote
    {
        get { return _usbStream.IsRemote; }
    }

    public int NumberOfComPorts
    {
        get { return Hardware.NumberOfComPorts; }
    }

    public int NumberOfIROutputPorts
    {
        get { return Hardware.NumberOfIROutputPorts; }
    }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            var switcher = outputSelector as IHandleInputSwitch ?? throw new NullReferenceException("outputSelector");

            Debug.Console(1,
                this,
                "Executing switch : '{0}' | '{1}' | '{2}'",
                inputSelector.ToString(),
                outputSelector.ToString(),
                signalType.ToString());

            switcher.HandleSwitch(inputSelector, signalType);
        }
        catch (Exception ex)
        {
            Debug.Console(1, this, "Error executing switch! : {0}", ex.Message);
        }
    }

    public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        var deviceBridge = new NvxDeviceBridge(this);
        deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
    }

    private void AddRoutingPorts()
    {
        SwitcherForHdmiOutput.AddRoutingPort(this);
        StreamInput.AddRoutingPort(this);
        SecondaryAudioInput.AddRoutingPort(this);
        SwitcherForAnalogAudioOutput.AddRoutingPort(this);
        SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
    }
}