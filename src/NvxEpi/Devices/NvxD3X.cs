using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Extensions;
using NvxEpi.Features.Audio;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Devices;

public class NvxD3X :
    NvxBaseDevice,
    INvxD3XDeviceWithHardware,
    IComPorts,
    IIROutputPorts,
    IHdmiOutput,
    IRoutingWithFeedback,
    IBasicVolumeWithFeedback
{
    private IBasicVolumeWithFeedback _audio;
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

        _audio = new NvxD3xAudio(hardware, this);
        _hdmiOutput = new HdmiOutput(this);

        Feedbacks.AddRange(new[] { (Feedback)_audio.MuteFeedback, _audio.VolumeLevelFeedback });

        Hardware.BaseEvent += (o, a) =>
        {
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
            if (outputSelector is not IHandleInputSwitch switcher)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "Unable to execute switch. OutputSelector is not IHandleInputSwitch {outputSelectorType}", this, outputSelector.ToString());
                return;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Switching {input} to {output} type {type}", inputSelector, outputSelector, signalType.ToString());

            if (inputSelector is null)
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Device is DmNvxD30. 'None' input not available", this);
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
        SwitcherForHdmiOutput.AddRoutingPort(this);
        StreamInput.AddRoutingPort(this);
        SecondaryAudioInput.AddRoutingPort(this);
        SwitcherForAnalogAudioOutput.AddRoutingPort(this);
        SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
    }

    public void VolumeUp(bool pressRelease)
    {
        _audio.VolumeUp(pressRelease);
    }

    public void VolumeDown(bool pressRelease)
    {
        _audio.VolumeDown(pressRelease);
    }

    public void MuteToggle()
    {
        _audio.MuteToggle();
    }

    public void SetVolume(ushort level)
    {
        _audio.SetVolume(level);
    }

    public void MuteOn()
    {
        _audio.MuteOn();
    }

    public void MuteOff()
    {
        _audio.MuteOff();
    }

    public IntFeedback VolumeLevelFeedback
    {
        get { return _audio.VolumeLevelFeedback; }
    }

    public BoolFeedback MuteFeedback
    {
        get { return _audio.MuteFeedback; }
    }
}