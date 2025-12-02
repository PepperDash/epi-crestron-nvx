using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Features.Audio;
using NvxEpi.Features.AutomaticRouting;
using NvxEpi.Features.Config;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Features.Streams.Usb;

using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

using HdmiInput = NvxEpi.Features.Hdmi.Input.HdmiInput;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NvxEpi.Extensions;
using NvxEpi.Abstractions.UsbcInput;
using NvxEpi.Features.Usbc.Input;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.InputSwitching;


namespace NvxEpi.Devices;

public class Nvx38X : 
    NvxBaseDevice, 
    IComPorts, 
    IIROutputPorts,
    IUsbStreamWithHardware, 
    IHdmiInput, 
    INvxUsbcInput,
    IVideowallMode, 
    IRoutingWithFeedback, 
    ICec,
    IBasicVolumeWithFeedback,
    ICurrentVideoInputWithUsbc
{
    private IBasicVolumeWithFeedback _audio;
    private IHdmiInput _hdmiInputs;
    private INvxUsbcInput _usbcInputs;
    private IVideowallMode _hdmiOutput;
    private IUsbStreamWithHardware _usbStream;
    private readonly NvxDeviceProperties _config;

    public event RouteChangedEventHandler RouteChanged;

    public Nvx38X(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter)
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

            if(Hardware is DmNvx38x nvx38x)
            {
                _audio = new Nvx38XAudio(nvx38x, this);
            }
            
            _usbStream = UsbStream.GetUsbStream(this, _config.Usb);
            _hdmiInputs = new HdmiInput(this);
            _usbcInputs = new UsbcInput(this);
            _hdmiOutput = new VideowallModeOutput(this);

            Feedbacks.AddRange(new [] { (Feedback)_audio.MuteFeedback, _audio.VolumeLevelFeedback });

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
            Debug.LogMessage(0, this, "Caught an exception in activate:{0}", ex);
            throw;
        }
    }

    public void ClearCurrentUsbRoute()
    {
        _usbStream.ClearCurrentUsbRoute();
    }

    public void MakeUsbRoute(IUsbStreamWithHardware hardware)
    {
        Debug.LogMessage(0, this, "Try Make USB Route for mac : {0}", hardware.UsbLocalId.StringValue);
        if (_usbStream is not UsbStream usbStream)
        {
            Debug.LogMessage(0, this, "cannot Make USB Route for url : {0} - UsbStream is null", hardware.UsbLocalId.StringValue);
            return;
        }
        usbStream.MakeUsbRoute(hardware);
    }

    public CrestronCollection<ComPort> ComPorts
    {
        get { return Hardware.ComPorts; }
    }

    public BoolFeedback DisabledByHdcp
    {
        get { return _hdmiOutput.DisabledByHdcp; }
    }

    public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
    {
        get 
        { 
            var combined = new Dictionary<uint, IntFeedback>();
            
            // Add HDMI inputs
            foreach (var kvp in _hdmiInputs.HdcpCapability)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            // Add USB-C inputs with offset based on HDMI collection size
            var hdmiCount = (uint)_hdmiInputs.HdcpCapability.Count;
            foreach (var kvp in _usbcInputs.HdcpCapability)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, IntFeedback>(combined);
        }
    }

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

    public IntFeedback VideoAspectRatioMode
    {
        get { return _hdmiOutput.VideoAspectRatioMode; }
    }

    public CrestronCollection<IROutputPort> IROutputPorts
    {
        get { return Hardware.IROutputPorts; }
    }

    public bool IsRemote
    {
        get { return _usbStream.IsRemote; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> UsbRemoteIds
    {
        get { return _usbStream.UsbRemoteIds; }
    }

    public int NumberOfComPorts
    {
        get { return Hardware.NumberOfComPorts; }
    }

    public int NumberOfIROutputPorts
    {
        get { return Hardware.NumberOfIROutputPorts; }
    }

    public Cec StreamCec
    {
        get { return Hardware.HdmiOut.StreamCec; }
    }

    public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
    {
        get 
        { 
            var combined = new Dictionary<uint, BoolFeedback>();
            
            foreach (var kvp in _hdmiInputs.SyncDetected)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.SyncDetected.Count;
            foreach (var kvp in _usbcInputs.SyncDetected)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, BoolFeedback>(combined);
        }
    }

    public ReadOnlyDictionary<uint, StringFeedback> CurrentResolution
    {
        get 
        { 
            var combined = new Dictionary<uint, StringFeedback>();
            
            foreach (var kvp in _hdmiInputs.CurrentResolution)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.CurrentResolution.Count;
            foreach (var kvp in _usbcInputs.CurrentResolution)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, StringFeedback>(combined);
        }
    }

    public ReadOnlyDictionary<uint, IntFeedback> AudioChannels 
    { 
        get 
        { 
            var combined = new Dictionary<uint, IntFeedback>();
            
            foreach (var kvp in _hdmiInputs.AudioChannels)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.AudioChannels.Count;
            foreach (var kvp in _usbcInputs.AudioChannels)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, IntFeedback>(combined);
        }
    }

    public ReadOnlyDictionary<uint, StringFeedback> AudioFormat 
    { 
        get 
        { 
            var combined = new Dictionary<uint, StringFeedback>();
            
            foreach (var kvp in _hdmiInputs.AudioFormat)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.AudioFormat.Count;
            foreach (var kvp in _usbcInputs.AudioFormat)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, StringFeedback>(combined);
        }
    }

    public ReadOnlyDictionary<uint, StringFeedback> ColorSpace 
    { 
        get 
        { 
            var combined = new Dictionary<uint, StringFeedback>();
            
            foreach (var kvp in _hdmiInputs.ColorSpace)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.ColorSpace.Count;
            foreach (var kvp in _usbcInputs.ColorSpace)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, StringFeedback>(combined);
        }
    }

    public ReadOnlyDictionary<uint, StringFeedback> HdrType 
    { 
        get 
        { 
            var combined = new Dictionary<uint, StringFeedback>();
            
            foreach (var kvp in _hdmiInputs.HdrType)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.HdrType.Count;
            foreach (var kvp in _usbcInputs.HdrType)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, StringFeedback>(combined);
        }
    }

    public IntFeedback VideowallMode
    {
        get { return _hdmiOutput.VideowallMode; }
    }

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            var switcher = outputSelector as IHandleInputSwitch ?? throw new NullReferenceException("outputSelector");

            Debug.LogInformation(
                this,
                "Executing switch : '{0}' | '{1}' | '{2}'",
                inputSelector?.ToString() ?? "{null}",
                outputSelector?.ToString() ?? "{null}",
                signalType.ToString());

            switcher.HandleSwitch(inputSelector, signalType);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(this, "Error executing switch!: {0}", ex);
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
        HdmiInput2Port.AddRoutingPort(this);
        UsbcInput1Port.AddRoutingPort(this);
        UsbcInput2Port.AddRoutingPort(this);
        SecondaryAudioInput.AddRoutingPort(this);
        AnalogAudioInput.AddRoutingPort(this);

        SwitcherForHdmiOutput.AddRoutingPort(this);
        SwitcherForSecondaryAudioOutput.AddRoutingPort(this);
        SwitcherForAnalogAudioOutput.AddRoutingPort(this);

        if (IsTransmitter)
        {
            SwitcherForStreamOutput.AddRoutingPort(this);
        }
        else
        {
            StreamInput.AddRoutingPort(this);
        }
    }

    public StringFeedback UsbLocalId
    {
        get { return _usbStream.UsbLocalId; }
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

    public ReadOnlyDictionary<uint, StringFeedback> HdcpCapabilityString 
    { 
        get 
        { 
            var combined = new Dictionary<uint, StringFeedback>();
            
            foreach (var kvp in _hdmiInputs.HdcpCapabilityString)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.HdcpCapabilityString.Count;
            foreach (var kvp in _usbcInputs.HdcpCapabilityString)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, StringFeedback>(combined);
        }
    }

    public ReadOnlyDictionary<uint, StringFeedback> HdcpSupport 
    { 
        get 
        { 
            var combined = new Dictionary<uint, StringFeedback>();
            
            foreach (var kvp in _hdmiInputs.HdcpSupport)
            {
                combined[kvp.Key] = kvp.Value;
            }
            
            var hdmiCount = (uint)_hdmiInputs.HdcpSupport.Count;
            foreach (var kvp in _usbcInputs.HdcpSupport)
            {
                combined[kvp.Key + hdmiCount] = kvp.Value;
            }
            
            return new ReadOnlyDictionary<uint, StringFeedback>(combined);
        }
    }        

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    DmNvx38x INvx38XHardware.Hardware => throw new NotImplementedException();
}