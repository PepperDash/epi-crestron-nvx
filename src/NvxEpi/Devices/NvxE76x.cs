using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.DmInput;
using NvxEpi.Extensions;
using NvxEpi.Features.Audio;
using NvxEpi.Features.AutomaticRouting;
using NvxEpi.Features.Config;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using System;
using System.Collections.Generic;
using DmInput = NvxEpi.Features.Dm.Input.DmInput;
using Feedback = PepperDash.Essentials.Core.Feedback;


namespace NvxEpi.Devices;

public class NvxE76x : 
    NvxBaseDevice, 
    IDmInput, 
    IRoutingWithFeedback, 
    IBasicVolumeWithFeedback
{
    private IBasicVolumeWithFeedback _audio;
    private IDmInput _dmInputs;
    private readonly NvxDeviceProperties _config;

    public event RouteChangedEventHandler RouteChanged;

    public NvxE76x(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter)
        : base(config, getHardware, isTransmitter)
    {
        _config = NvxDeviceProperties.FromDeviceConfig(config);
        AddPreActivationAction(AddRoutingPorts);
    }

    public override bool CustomActivate()
    {
        try
        {
            var result = base.CustomActivate();

            _audio = new NvxE760xAudio(Hardware as DmNvxE760x, this);
            _dmInputs = new DmInput(this);

            Feedbacks.AddRange(new [] { (Feedback)_audio.MuteFeedback, _audio.VolumeLevelFeedback });

            if (_config.EnableAutoRoute)
                // ReSharper disable once ObjectCreationAsStatement
                //new AutomaticInputRouter(_dmInputs);

            AddMcMessengers();

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
        catch (Exception ex)
        {
            Debug.Console(0, this, "Caught an exception in activate:{0}", ex);
            throw;
        }
    }
    /*
    public void ClearCurrentUsbRoute()
    {
        _usbStream.ClearCurrentUsbRoute();
    }

    public void MakeUsbRoute(IUsbStreamWithHardware hardware)
    {
        Debug.Console(0, this, "Try Make USB Route for mac : {0}", hardware.UsbLocalId.StringValue);
        if (_usbStream is not UsbStream usbStream)
        {
            Debug.Console(0, this, "cannot Make USB Route for url : {0} - UsbStream is null", hardware.UsbLocalId.StringValue);
            return;
        }
        usbStream.MakeUsbRoute(hardware);
    }
    */
    
    public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
    {
        get { return _dmInputs.HdcpCapability; }
    }


    public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
    {
        get { return _dmInputs.SyncDetected; }
    }

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            var switcher = outputSelector as IHandleInputSwitch ?? throw new NullReferenceException("outputSelector");

            Debug.Console(1,
                this,
                "Executing switch : '{0}' | '{1}' | '{2}'",
                inputSelector?.ToString() ?? "{null}",
                outputSelector?.ToString() ?? "{null}",
                signalType.ToString());

            switcher.HandleSwitch(inputSelector, signalType);
        }
        catch (Exception ex)
        {
            Debug.Console(1, this, "Error executing switch!: {0}", ex);
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
        AnalogAudioInput.AddRoutingPort(this);

        if (IsTransmitter)
        {
            SwitcherForStreamOutput.AddRoutingPort(this);
        }

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

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();
}