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
using NvxEpi.McMessengers;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using ScreenInfo = PepperDash.Essentials.Core.DeviceTypeInterfaces.ScreenInfo;
using LayoutInfo = PepperDash.Essentials.Core.DeviceTypeInterfaces.LayoutInfo;
using WindowConfig = PepperDash.Essentials.Core.DeviceTypeInterfaces.WindowConfig;


namespace NvxEpi.Devices;

public class Nvx38X :
    NvxBaseDevice,
    IComPorts,
    IIROutputPorts,
    IUsbStreamWithHardware,
    IHdmiInput,
    IVideowallMode,
    IRoutingWithFeedback,
    ICec,
    IBasicVolumeWithFeedback,
    IHasScreensWithLayouts
{
    private IBasicVolumeWithFeedback _audio;
    private IHdmiInput _hdmiInputs;
    private IVideowallMode _hdmiOutput;
    private IUsbStreamWithHardware _usbStream;
    private readonly NvxDeviceProperties _config;
    private DmNvx38x _device;
    private readonly DmNvxMultiviewControlsSetup _deviceControls;
    public Dictionary<uint, ScreenInfo> Screens { get; private set; }
    public FeedbackCollection<StringFeedback> ScreenNamesFeedbacks { get; private set; }
    public FeedbackCollection<BoolFeedback> ScreenEnablesFeedbacks { get; private set; }
    public FeedbackCollection<StringFeedback> LayoutNamesFeedbacks { get; private set; }
    private Dictionary<uint, string> LayoutNames { get; set; }

    private readonly Dictionary<uint, Nvx38xLayouts> _screenLayouts = new Dictionary<uint, Nvx38xLayouts>();

    public event RouteChangedEventHandler RouteChanged;

    public Nvx38X(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter)
        : base(config, getHardware, isTransmitter)
    {
        _config = NvxDeviceProperties.FromDeviceConfig(config);
        AddPreActivationAction(AddRoutingPorts);
        
        // Initialize multiview controls and screen layout dictionaries
        _screenLayouts = new Dictionary<uint, Nvx38xLayouts>();
        LayoutNames = new Dictionary<uint, string>();
        
        AddPreActivationAction(InitializeMultiviewFeatures);
    }

    private void InitializeMultiviewFeatures()
    {
        // Initialize screens dictionary - DM-NVX-384 supports a single screen output
        Screens = new Dictionary<uint, ScreenInfo>();
        ScreenNamesFeedbacks = new FeedbackCollection<StringFeedback>();
        ScreenEnablesFeedbacks = new FeedbackCollection<BoolFeedback>();
        LayoutNamesFeedbacks = new FeedbackCollection<StringFeedback>();

        // Initialize default screen configuration for DM-NVX-38X
        var mainScreen = new ScreenInfo
        {
            Name = "Main Display",
            Enabled = true,
            Layouts = new Dictionary<uint, LayoutInfo>
            {               
                [1] = new LayoutInfo
                {
                    LayoutIndex = 0,
                    LayoutName = "Full Screen",
                    LayoutType = "fullScreen",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Full Screen", Input = "Input1" }
                    }
                },
                [2] = new LayoutInfo { LayoutIndex = 201, LayoutName = "Side By Side", LayoutType = "sideBySide", Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" }
                    }
                },
                [3] = new LayoutInfo
                {
                    LayoutIndex = 202,
                    LayoutName = "PIP Small Top Left",
                    LayoutType = "pipSmallTopLeft",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" }
                    }
                },
                [4] = new LayoutInfo
                {
                    LayoutIndex = 203,
                    LayoutName = "PIP Small Top Right",
                    LayoutType = "pipSmallTopRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" }
                    }
                },
                [5] = new LayoutInfo
                {
                    LayoutIndex = 204,
                    LayoutName = "PIP Small Bottom Left",
                    LayoutType = "pipSmallBottomLeft",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" }
                    }
                },
                [6] = new LayoutInfo
                {
                    LayoutIndex = 205,
                    LayoutName = "PIP Small Bottom Right",
                    LayoutType = "pipSmallBottomRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" }
                    }
                },
                [7] = new LayoutInfo
                {
                    LayoutIndex = 301,
                    LayoutName = "1 Top, 2 Bottom",
                    LayoutType = "oneTopTwoBottom",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" }
                    }
                },
                [8] = new LayoutInfo
                {
                    LayoutIndex = 302,
                    LayoutName = "2 Top, 1 Bottom",
                    LayoutType = "twoTopOneBottom",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" }
                    }
                },
                [9] = new LayoutInfo
                {
                    LayoutIndex = 303,
                    LayoutName = "1 Left, 2 Right",
                    LayoutType = "oneLeftTwoRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" }
                    }
                },
                [10] = new LayoutInfo
                {
                    LayoutIndex = 401,
                    LayoutName = "2 Top, 2 Bottom",
                    LayoutType = "twoTopTwoBottom",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" }
                    }
                },
                [11] = new LayoutInfo
                {
                    LayoutIndex = 402,
                    LayoutName = "1 Left, 3 Right",
                    LayoutType = "oneLeftThreeRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" }
                    }
                },
                [12] = new LayoutInfo
                {
                    LayoutIndex = 501,
                    LayoutName = "1 Large Left, 4 Right",
                    LayoutType = "oneLargeLeftFourRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" }
                    }
                },
                [13] = new LayoutInfo
                {
                    LayoutIndex = 502,
                    LayoutName = "4 Left, 1 Large Right",
                    LayoutType = "fourLeftOneLargeRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" }
                    }
                },
                [14] = new LayoutInfo
                {
                    LayoutIndex = 503,
                    LayoutName = "1 Large Left, 4 Right",
                    LayoutType = "oneLargeLeftFourRight",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" }
                    }
                },
                [15] = new LayoutInfo
                {
                    LayoutIndex = 601,
                    LayoutName = "3 Top, 3 Bottom",
                    LayoutType = "threeTopThreeBottom",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" },
                        [6] = new WindowConfig { Label = "Window 6", Input = "Input6" }
                    }
                },
                [16] = new LayoutInfo
                {
                    LayoutIndex = 602,
                    LayoutName = "1 Large Left, 5 Stacked",
                    LayoutType = "oneLargeLeftFiveStacked",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" },
                        [6] = new WindowConfig { Label = "Window 6", Input = "Input6" }
                    }
                },
                [17] = new LayoutInfo
                {
                    LayoutIndex = 603,
                    LayoutName = "5 Around, 1 Large Bottom Left",
                    LayoutType = "fiveAroundOneLargeBottomLeft",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" },
                        [6] = new WindowConfig { Label = "Window 6", Input = "Input6" }
                    }
                },
                [18] = new LayoutInfo
                {
                    LayoutIndex = 604,
                    LayoutName = "5 Around, 1 Large Top Left",
                    LayoutType = "fiveAroundOneLargeTopLeft",
                    Windows = new Dictionary<uint, WindowConfig>
                    {
                        [1] = new WindowConfig { Label = "Window 1", Input = "Input1" },
                        [2] = new WindowConfig { Label = "Window 2", Input = "Input2" },
                        [3] = new WindowConfig { Label = "Window 3", Input = "Input3" },
                        [4] = new WindowConfig { Label = "Window 4", Input = "Input4" },
                        [5] = new WindowConfig { Label = "Window 5", Input = "Input5" },
                        [6] = new WindowConfig { Label = "Window 6", Input = "Input6" }
                    }
                },
            }
        };

        Screens.Add(1, mainScreen);

        // Initialize feedbacks
        ScreenNamesFeedbacks.Add(new StringFeedback($"ScreenName{1}", () => mainScreen.Name));
        ScreenEnablesFeedbacks.Add(new BoolFeedback($"ScreenEnabled{1}", () => mainScreen.Enabled));

        // Initialize layout name feedbacks
        foreach (var layout in mainScreen.Layouts)
        {
            LayoutNames.Add(layout.Key, layout.Value.LayoutName);
            LayoutNamesFeedbacks.Add(new StringFeedback($"LayoutName{layout.Key}", () => layout.Value.LayoutName));
        }
    }

    public override bool CustomActivate()
    {
        try
        {
            var result = base.CustomActivate();

            if (Hardware is DmNvx38x nvx38x)
            {
                _audio = new Nvx38XAudio(nvx38x, this);
                // Initialize the multiview hardware reference
                _device = nvx38x;
            }

            _usbStream = UsbStream.GetUsbStream(this, _config.Usb);
            _hdmiInputs = new HdmiInput(this);
            _hdmiOutput = new VideowallModeOutput(this);

            Feedbacks.AddRange(new[] { (Feedback)_audio.MuteFeedback, _audio.VolumeLevelFeedback });

            if (_config.EnableAutoRoute)
                // ReSharper disable once ObjectCreationAsStatement
                new AutomaticInputRouter(_hdmiInputs);

            AddMcMessengers();

            // Add the IHasScreensWithLayouts messenger for multiview support
            var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();
            if (mc != null)
            {
                var screensMessenger = new IHasScreensWithLayoutsMessenger($"{Key}-screens", $"/device/{Key}", this);
                mc.AddDeviceMessenger(screensMessenger);
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Added IHasScreensWithLayouts messenger for {0}", this, Key);
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

        return result;
    }
    catch (Exception ex)
    {
        Debug.LogMessage(0, this, "Caught an exception in activate:{0}", ex);
        throw;
    }
}    public void ClearCurrentUsbRoute()
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
        get { return _hdmiInputs.HdcpCapability; }
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
        get { return _hdmiInputs.SyncDetected; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> CurrentResolution
    {
        get { return _hdmiInputs.CurrentResolution; }
    }

    public ReadOnlyDictionary<uint, IntFeedback> AudioChannels { get { return _hdmiInputs.AudioChannels; } }

    public ReadOnlyDictionary<uint, StringFeedback> AudioFormat { get { return _hdmiInputs.AudioFormat; } }

    public ReadOnlyDictionary<uint, StringFeedback> ColorSpace { get { return _hdmiInputs.ColorSpace; } }

    public ReadOnlyDictionary<uint, StringFeedback> HdrType { get { return _hdmiInputs.HdrType; } }

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

    public ReadOnlyDictionary<uint, StringFeedback> HdcpCapabilityString { get { return _hdmiInputs.HdcpCapabilityString; } }

    public ReadOnlyDictionary<uint, StringFeedback> HdcpSupport { get { return _hdmiInputs.HdcpSupport; } }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    #region Methods        

    /// <summary>
    /// Set the default window routes for the DM-NVX-384.
    /// </summary>
    public void DefaultWindowRoutes()
    {
        // For DM-NVX-384, we need to implement the proper multiview API
        // This is a placeholder implementation that logs the action
        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Setting default window routes for DM-NVX-384", this);
        
        // TODO: Replace with actual DM-NVX-384 multiview API calls
        
        // Placeholder: Apply default layout (full screen with first input)
        if (_device != null)
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "DM-NVX-384 hardware initialized", this);
        }
    }

    /// <summary>
    /// Change the current window layout using values from 0-6. 
    /// </summary>
    /// <param name="layout">Values from 0 - 6</param>
    public void SetWindowLayout(uint layout)
    {
        WindowLayout.eLayoutType _layoutType;
        switch (layout)
        {
            case 0:
                _layoutType = WindowLayout.eLayoutType.Automatic;
                break;
            case 1:
                _layoutType = WindowLayout.eLayoutType.Fullscreen;
                break;
            case 2:
                _layoutType = WindowLayout.eLayoutType.PictureInPicture;
                break;
            case 3:
                _layoutType = WindowLayout.eLayoutType.SideBySide;
                break;
            case 4:
                _layoutType = WindowLayout.eLayoutType.ThreeUp;
                break;
            case 5:
                _layoutType = WindowLayout.eLayoutType.Quadview;
                break;
            case 6:
                _layoutType = WindowLayout.eLayoutType.ThreeSmallOneLarge;
                break;
            default:
                Debug.LogDebug(this, "Invalid layout value: {0}. Valid range 0 - 6.", layout);
                return;
        }
        
        // TODO: Replace with actual DM-NVX-384 multiview API calls
        // _HdWpChassis.HdWpWindowLayout.Layout = _layoutType;
        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Setting layout {0} for DM-NVX-384", this, _layoutType);

        //Reset AV Routes when SetWindowLayout is called
        //DefaultWindowRoutes();
    }

    /// <summary>
    /// Set the window layout using the WindowLayout.eLayoutType enum.
    /// </summary>
    /// <param name="layout"></param>
    public void SetWindowLayout(WindowLayout.eLayoutType layout)
    {
        // TODO: Replace with actual DM-NVX-384 multiview API calls  
        // _HdWpChassis.HdWpWindowLayout.Layout = layout;
        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Setting layout {0} for DM-NVX-384", this, layout);

        //Reset AV Routes when SetWindowLayout is called
        DefaultWindowRoutes();
    }


    /// <summary>
    /// Apply a specific layout to a screen by its ID and layout index.
    /// </summary>
    /// <param name="screenId"></param>
    /// <param name="layoutIndex"></param>
    public void ApplyLayout(uint screenId, uint layoutIndex)
    {
        if (!Screens.TryGetValue(screenId, out var screen))
        {
            Debug.LogError(this, $"[ApplyLayout] Screen '{screenId}' not found.");
            return;
        }

        if (!screen.Layouts.TryGetValue(layoutIndex, out var layout))
        {
            Debug.LogError(this, $"[ApplyLayout] Layout '{layoutIndex}' not found for screen '{screenId}'.");
            return;
        }

        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, 
            "[ApplyLayout] Applying layout '{0}' ({1}) to screen '{2}' for DM-NVX-384", 
            this, layout.LayoutName, layout.LayoutType, screenId);

        foreach (var window in layout.Windows)
        {
            uint windowId = window.Key;
            string inputKey = window.Value.Input?.ToLower();

            if (string.IsNullOrEmpty(inputKey))
            {
                Debug.LogError(this, $"[ApplyLayout] Missing input for window {windowId}.");
                continue;
            }

            WindowLayout.eVideoSourceType? source = null;
            switch (inputKey)
            {
                case "input1":
                    source = WindowLayout.eVideoSourceType.Input1;
                    break;
                case "input2":
                    source = WindowLayout.eVideoSourceType.Input2;
                    break;
                case "input3":
                    source = WindowLayout.eVideoSourceType.Input3;
                    break;
                case "input4":
                    source = WindowLayout.eVideoSourceType.Input4;
                    break;
            }

            if (source.HasValue)
            {
                // TODO: Replace with actual DM-NVX-384 multiview API calls
                // _HdWpChassis.HdWpWindowLayout.SetVideoSource(windowId, source.Value);
                Debug.LogVerbose(this, $"[ApplyLayout] Set window {windowId} to {inputKey} ({window.Value.Label}).");
            }
            else
            {
                Debug.LogError(this, $"[ApplyLayout] Invalid input '{inputKey}' for window {windowId}.");
            }
        }

        SetWindowLayout((uint)layout.LayoutIndex);

        // Send layout data via messenger
        var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();
        if (mc != null)
        {
            var messenger = new IHasScreensWithLayoutsMessenger($"{Key}-screens", $"/device/{Key}", this);
            mc.AddDeviceMessenger(messenger);
            messenger.SendCurrentLayoutStatus(screenId, layout);
        }
    }
    #endregion

    #region Layouts
    
    class Nvx38xLayouts : ISelectableItems<uint>, IKeyName
    {
        private Dictionary<uint, ISelectableItem> _items = new Dictionary<uint, ISelectableItem>();
        public Dictionary<uint, ISelectableItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                ItemsUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        private uint _currentItem;
        public uint CurrentItem
        {
            get => _currentItem;
            set
            {
                _currentItem = value;
                CurrentItemChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string Name { get; private set; }

        public string Key { get; private set; }

        public event EventHandler ItemsUpdated;
        public event EventHandler CurrentItemChanged;

        /// <summary>
        /// Constructor for the Nvx38x layouts, full parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        public Nvx38xLayouts(string key, string name, Dictionary<uint, ISelectableItem> items)
        {
            Items = items;
            Key = key;
            Name = name;
        }

        public class Nvx38xLayout : ISelectableItem
        {
            public string Key { get; private set; }
            public string Name { get; private set; }

            private DmNvx38x _parent;

            private bool _isSelected;

            public int Id { get; set; }
            public bool IsSelected
            {
                get { return _isSelected; }
                set
                {
                    if (_isSelected == value) return;
                    _isSelected = value;
                    var handler = ItemUpdated;
                    if (handler != null)
                        handler(this, EventArgs.Empty);
                }
            }

            private readonly int screenIndex;

            public event EventHandler ItemUpdated;

            /// <summary>
            /// Constructor for the Nvx38x layout, full parameters
            /// </summary>
            /// <param name="key"></param>
            /// <param name="name"></param>
            /// <param name="screenIndex"></param>
            /// <param name="id"></param>
            /// <param name="parent"></param>
            public Nvx38xLayout(string key, string name, int screenIndex, int id, DmNvx38x parent)
            {
                Key = key;
                Name = name;
                this.screenIndex = screenIndex;
                Id = id;
                _parent = parent;
            }

            public void Select()
            {
                // The parent should be the Nvx38X device, not the hardware
                // Cast _parent to Nvx38X and call ApplyLayout
                if (_parent is DmNvx38x)
                {
                    // We need to get the actual Nvx38X device instance
                    var nvx38xDevice = DeviceManager.AllDevices.OfType<Nvx38X>()
                        .FirstOrDefault(d => d._device == _parent);
                    
                    if (nvx38xDevice != null)
                    {
                        nvx38xDevice.ApplyLayout((uint)screenIndex, (uint)Id);
                    }
                    else
                    {
                        Debug.LogError("Could not find Nvx38X device for layout selection");
                    }
                }
            }
        }
    }
    #endregion
}