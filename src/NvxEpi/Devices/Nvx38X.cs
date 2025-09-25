using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Extensions;
using NvxEpi.Features.Audio;
using NvxEpi.Features.AutomaticRouting;
using NvxEpi.Features.Config;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Features.Streams.Usb;
using NvxEpi.JoinMaps;
using NvxEpi.McMessengers;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.WindowLayout;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using static Crestron.SimplSharpPro.Lighting.ZumWired.ZumNetBridgeRoom.ZumWiredRoomInterface;
using Feedback = PepperDash.Essentials.Core.Feedback;
using HdmiInput = NvxEpi.Features.Hdmi.Input.HdmiInput;
using LayoutInfo = PepperDash.Essentials.Core.DeviceTypeInterfaces.LayoutInfo;
using ScreenInfo = PepperDash.Essentials.Core.DeviceTypeInterfaces.ScreenInfo;
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
    #region Fields, Properties, and Events

    private IBasicVolumeWithFeedback _audio;
    private IHdmiInput _hdmiInputs;
    private IVideowallMode _hdmiOutput;
    private IUsbStreamWithHardware _usbStream;
    private readonly NvxDeviceProperties _config;
    private DmNvx38x _device;
    private readonly Dictionary<uint, Nvx38xLayouts> _screenLayouts = new Dictionary<uint, Nvx38xLayouts>();
    
    public Dictionary<uint, ScreenInfo> Screens { get; private set; }
    public FeedbackCollection<StringFeedback> ScreenNamesFeedbacks { get; private set; }
    public FeedbackCollection<BoolFeedback> ScreenEnablesFeedbacks { get; private set; }
    public FeedbackCollection<StringFeedback> LayoutNamesFeedbacks { get; private set; }
    private Dictionary<uint, string> LayoutNames { get; set; }
    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();
    public BoolFeedback MultiviewEnabledFeedback { get; private set; }
    public BoolFeedback MultiviewDisabledFeedback { get; private set; }
    public IntFeedback MultiviewLayoutFeedback { get; private set; }
    public IntFeedback MultiviewAudioSourceFeedback { get; private set; }
    public FeedbackCollection<StringFeedback> MultiviewWindowStreamUrlFeedbacks { get; private set; }
    public FeedbackCollection<StringFeedback> MultiviewWindowLabelFeedbacks { get; private set; }

    public event RouteChangedEventHandler RouteChanged;
    public event GenericEventHandler WindowLayoutChanged;

    #endregion

    #region Constructor

    public Nvx38X(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter, Nvx38xMultiviewConfig multiviewProps)
        : base(config, getHardware, isTransmitter)
    {
        _config = NvxDeviceProperties.FromDeviceConfig(config);
        AddPreActivationAction(AddRoutingPorts);        

        // Initialize multiview controls and screen layout dictionaries
        LayoutNames = new Dictionary<uint, string>();

        Screens = new Dictionary<uint, ScreenInfo>(multiviewProps.Screens);

        ScreenNamesFeedbacks = new FeedbackCollection<StringFeedback>();
        ScreenEnablesFeedbacks = new FeedbackCollection<BoolFeedback>();
        LayoutNamesFeedbacks = new FeedbackCollection<StringFeedback>();
        LayoutNames = new Dictionary<uint, string>();
        MultiviewWindowStreamUrlFeedbacks = new FeedbackCollection<StringFeedback>();
        MultiviewWindowLabelFeedbacks = new FeedbackCollection<StringFeedback>();

        AddPreActivationAction(InitializeMultiviewFeatures);

        foreach (var item in Screens)
        {
            var _layouts = new Dictionary<uint, ISelectableItem>();
            var screen = item.Value;
            var screenKey = item.Key;

            ScreenNamesFeedbacks.Add(new StringFeedback("ScreenName-" + screenKey, () => screen.Name));
            ScreenEnablesFeedbacks.Add(new BoolFeedback("ScreenEnable-" + screenKey, () => screen.Enabled));
            LayoutNamesFeedbacks.Add(new StringFeedback("LayoutNames-" + screenKey, () => LayoutNames[screenKey]));

            foreach (var layout in screen.Layouts)
            {
                LayoutNames[screenKey] = layout.Value.LayoutName;
                _layouts.Add(layout.Key, new Nvx38xLayouts.Nvx38xLayout($"{layout.Key}", layout.Value.LayoutName, screen.ScreenIndex, (int)layout.Key, this));
            }

            _screenLayouts[screenKey] = new Nvx38xLayouts($"{Key}-screen-{screenKey}", $"{Key}-screen-{screenKey}", _layouts);
            DeviceManager.AddDevice(_screenLayouts[screenKey]); //Add to device manager which will show up in devlist
        }

        AddPostActivationAction(AddFeedbackCollections);
    }

    public Nvx38X(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter) : base(config, getHardware, isTransmitter)
    {
    }

    #endregion

    #region Generic Methods

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

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            var switcher = outputSelector as IHandleInputSwitch ?? throw new NullReferenceException("outputSelector");

            Debug.LogDebug(
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

    #endregion

    #region Interface Properties

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

    public StringFeedback UsbLocalId
    {
        get { return _usbStream.UsbLocalId; }
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

    #endregion

    #region InitializeMultiviewFeatures

    private void InitializeMultiviewFeatures()
    {
       
        MultiviewEnabledFeedback = new BoolFeedback("MultiviewEnabledFeedback", () => _device.MultiviewControlsSetup.EnabledFeedback.BoolValue);
        MultiviewDisabledFeedback = new BoolFeedback("MultiviewDisabledFeedback", () => _device.MultiviewControlsSetup.DisabledFeedback.BoolValue);
        MultiviewLayoutFeedback = new IntFeedback("MultiviewLayoutFeedback", () => _device.MultiviewControlsSetup.MultiViewWindowControls.LayoutFeedback.UShortValue);
        MultiviewAudioSourceFeedback = new IntFeedback("MultiviewAudioSourceFeedback", () => _device.MultiviewControlsSetup.MultiViewWindowControls.AudioSourceFeedback.UShortValue);

        // Initialize multiview window feedbacks
        for (uint windowId = 1; windowId <= 6; windowId++)
        {
            var windowIndex = (int)windowId - 1;

            MultiviewWindowStreamUrlFeedbacks.Add(new StringFeedback(
                $"MultiviewWindow{windowId}StreamUrl",
                () => GetWindowStreamUrl(windowId)));

            MultiviewWindowLabelFeedbacks.Add(new StringFeedback(
                $"MultiviewWindow{windowId}Label",
                () => GetWindowLabel(windowId)));
        }

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
                [2] = new LayoutInfo
                {
                    LayoutIndex = 201,
                    LayoutName = "Side By Side",
                    LayoutType = "sideBySide",
                    Windows = new Dictionary<uint, WindowConfig>
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

    #endregion

    #region Override Method - CustomActivate

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
    }

    #endregion

    #region PostActivate

    public void AddFeedbackCollections()
    {    
        AddCollectionsToList(ScreenNamesFeedbacks, LayoutNamesFeedbacks);
        AddCollectionsToList(ScreenEnablesFeedbacks);
        AddCollectionsToList(MultiviewWindowStreamUrlFeedbacks, MultiviewWindowLabelFeedbacks);
    }

    #endregion

    #region Override Method - LinkToApi (Bridge Linking)

    public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        // Use the specialized multiview joinMap for NVX-38X
        var joinMap = new Nvx38xMultiviewJoinMap(joinStart);

        // Register the joinMap with the bridge
        bridge?.AddJoinMap(Key, joinMap);

        // Link the base NVX functionality
        var deviceBridge = new NvxDeviceBridge(this);
        deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);

        // Link the multiview-specific functionality
        LinkMultiviewControls(trilist, joinMap);
    }

    private void LinkMultiviewControls(BasicTriList trilist, Nvx38xMultiviewJoinMap joinMap)
    {
        try
        {
            Debug.LogDebug(this, "Linking multiview controls for DM-NVX-38x");

            // Multiview Controls: Enter
            trilist.SetBoolSigAction(joinMap.MultiviewEnter.JoinNumber, value =>
            {
                if (value)
                {
                    if (_device != null &&
                        _device.MultiviewControlsSetup != null &&
                        _device.MultiviewControlsSetup.MultiViewWindowControls != null &&
                        _device.MultiviewControlsSetup.MultiViewWindowControls.Enter != null)
                    {
                        _device.MultiviewControlsSetup.MultiViewWindowControls.Enter.BoolValue = true;
                        Debug.LogDebug(this, "Multiview Enter triggered");
                    }
                }
                else
                    if (_device != null &&
                        _device.MultiviewControlsSetup != null &&
                        _device.MultiviewControlsSetup.MultiViewWindowControls != null &&
                        _device.MultiviewControlsSetup.MultiViewWindowControls.Enter != null)
                {
                    _device.MultiviewControlsSetup.MultiViewWindowControls.Enter.BoolValue = false;
                    Debug.LogDebug(this, "Multiview Enter released");
                }
            });

            // Multiview Controls: Enable Press
            trilist.SetBoolSigAction(joinMap.MultiviewEnabled.JoinNumber, value =>
            {
                if (value)
                {
                    _device?.MultiviewControlsSetup?.Enable();
                    Debug.LogDebug(this, "Multiview Enable triggered");
                }
            });

            // Multiview Controls: Enable Feedback
            MultiviewEnabledFeedback?.LinkInputSig(trilist.BooleanInput[joinMap.MultiviewEnabled.JoinNumber]);

            // Multiview Controls: Disable Press
            trilist.SetBoolSigAction(joinMap.MultiviewDisabled.JoinNumber, value =>
            {
                if (value)
                {
                    _device?.MultiviewControlsSetup?.Disable();
                    Debug.LogDebug(this, "Multiview Disable triggered");
                }
            });

            // Multiview Controls: Disabled Feedback
            MultiviewDisabledFeedback?.LinkInputSig(trilist.BooleanInput[joinMap.MultiviewDisabled.JoinNumber]);

            // Multiview Controls: Set Layout
            trilist.SetUShortSigAction(joinMap.MultiviewLayout.JoinNumber, layoutValue =>
            {
                if (layoutValue >= 1 && layoutValue <= 18)
                {
                    SetWindowLayout(layoutValue);
                    Debug.LogDebug(this, "Multiview layout set to: {0}", layoutValue);
                }
            });

            // Multiview Controls: Layout Feedback
            MultiviewLayoutFeedback?.LinkInputSig(trilist.UShortInput[joinMap.MultiviewLayoutFeedback.JoinNumber]);

            // Multiview Controls: Set Audio Source
            trilist.SetUShortSigAction(joinMap.MultiviewAudioSource.JoinNumber, audioSource =>
            {   
                if (_device != null &&
                    _device.MultiviewControlsSetup != null &&
                    _device.MultiviewControlsSetup.MultiViewWindowControls != null &&
                    _device.MultiviewControlsSetup.MultiViewWindowControls.AudioSource != null)
                {
                    _device.MultiviewControlsSetup.MultiViewWindowControls.AudioSource.UShortValue = audioSource;
                    Debug.LogDebug(this, "Multiview audio source set to: {0}", audioSource);
                }
            });

            // Multiview Controls: Audio Source Feedback
            MultiviewAudioSourceFeedback?.LinkInputSig(trilist.UShortInput[joinMap.MultiviewAudioSourceFeedback.JoinNumber]);

            // Window Stream URL Controls and Feedbacks
            for (uint windowId = 1; windowId <= 6; windowId++)
            {
                var windowIndex = windowId; // Capture for closure

                // Stream URL Control
                var streamUrlJoin = joinMap.GetWindowStreamUrlJoin(windowIndex);
                trilist.SetStringSigAction(streamUrlJoin.JoinNumber, streamUrl =>
                {
                    SetWindowVideoSourceStreamUrl(windowIndex, streamUrl);
                    Debug.LogDebug(this, "Set window {0} stream URL to: {1}", windowIndex, streamUrl);
                });

                // Stream URL Feedback
                var streamUrlFeedbackJoin = joinMap.GetWindowStreamUrlFeedbackJoin(windowIndex);
                if (windowIndex <= MultiviewWindowStreamUrlFeedbacks?.Count)
                {
                    MultiviewWindowStreamUrlFeedbacks[(int)windowIndex - 1]?.LinkInputSig(
                        trilist.StringInput[streamUrlFeedbackJoin.JoinNumber]);
                }

                // Window Label Control
                var labelJoin = joinMap.GetWindowLabelJoin(windowIndex);
                trilist.SetStringSigAction(labelJoin.JoinNumber, label =>
                {
                    SetWindowLabel(windowIndex, label);
                    Debug.LogDebug(this, "Set window {0} label to: {1}", windowIndex, label);
                });

                // Window Label Feedback
                var labelFeedbackJoin = joinMap.GetWindowLabelFeedbackJoin(windowIndex);
                if (windowIndex <= MultiviewWindowLabelFeedbacks?.Count)
                {
                    MultiviewWindowLabelFeedbacks[(int)windowIndex - 1]?.LinkInputSig(
                        trilist.StringInput[labelFeedbackJoin.JoinNumber]);
                }
            }
            Debug.LogDebug(this, "Successfully linked multiview controls for DM-NVX-38x");
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error linking multiview controls: {0}", ex.Message);
        }
    }

    #endregion

    #region AddRoutingPorts

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

    #endregion

    #region Multiview Methods

    /// <summary>
    /// Change the current window layout using values from 1-18. Call from ApplyLayout() only.
    /// </summary>
    /// <param name="layout">Values from 1 - 18</param>
    private void SetWindowLayout(uint layout)
    {
        Nvx38xWindowLayout.ExtendedLayoutType _layoutType;
        switch (layout)
        {
            case 1:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.Fullscreen;
                break;
            case 2:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.SideBySide;
                break;
            case 3:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.PipSmallTopLeft;
                break;
            case 4:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.PipSmallTopRight;
                break;
            case 5:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.PipSmallBottomLeft;
                break;
            case 6:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.PipSmallBottomRight;
                break;
            case 7:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.OneTopTwoBottom;
                break;
            case 8:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.TwoTopOneBottom;
                break;
            case 9:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.OneLeftTwoRight;
                break;
            case 10:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.TwoTopTwoBottom;
                break;
            case 11:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.OneLeftThreeRight;
                break;
            case 12:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.OneLargeLeftFourRight;
                break;
            case 13:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.FourLeftOneLargeRight;
                break;
            case 14:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.TwoLeftOneLargeCenterTwoRight;
                break;
            case 15:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.ThreeTopThreeBottom;
                break;
            case 16:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.OneLargeLeftFiveStacked;
                break;
            case 17:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.FiveAroundOneLargeBottomLeft;
                break;
            case 18:
                _layoutType = Nvx38xWindowLayout.ExtendedLayoutType.FiveAroundOneLargeTopLeft;
                break;
            default:
                Debug.LogDebug(this, "Invalid layout value: {0}. Valid range 1 - 18.", layout);
                return;
        }
        var layoutSig = _device.MultiviewControlsSetup.MultiViewWindowControls.Layout;
        layoutSig.UShortValue = (ushort)_layoutType;
        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Set layout {0} for DM-NVX-38x", this, layout);
    }

    /// <summary>
    /// Set the window layout using the Nvx38xWindowLayout.ExtendedLayoutType enum.
    /// </summary>
    /// <param name="layout"></param>
    public void SetWindowLayout(Nvx38xWindowLayout.ExtendedLayoutType layout)
    {
        if(Screens != null)
        {
            try
            {
                Nvx38xWindowLayout.ExtendedLayoutType _layoutType = layout;
                Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Set ExtendedLayoutType {0} for DM-NVX-38x", this, layout);
            }
            catch (Exception ex)
            {
                Debug.LogError(this, "Error setting ExtendedLayoutType for DM-NVX-38x: {0}", ex.Message);
            }
        }
        else
        {
            Debug.LogError(this, "DM-NVX-38x multiview hardware not available");
        }
    }

    /// <summary>
    /// Set video source for a specific window in the multiview layout.
    /// </summary>
    /// <param name="windowId">Window ID (1-6)</param>
    /// <param name="sourceType">Video source type</param>
    public void SetWindowVideoSource(uint windowId, WindowLayout.eVideoSourceType sourceType)
    {
        if (Screens != null)
        {
            // throw not implemented exception as device does not support this method
            throw new NotImplementedException("SetWindowVideoSource with WindowLayout.eVideoSourceType is not implemented for DM-NVX-38x");
        }
        else
        {
            Debug.LogError(this, "DM-NVX-38x multiview hardware not available");
        }
    }

    /// <summary>
    /// Set video source for a specific window in the multiview layout. What calls this method?
    /// </summary>
    /// <param name="windowId">Window ID (1-6)</param>
    /// <param name="sourceType">Video source type</param>
    public void SetWindowVideoSourceStreamUrl(uint windowId, string streamUrl)
    {
        try
        {
            if (windowId < 1 || windowId > 6)
            {
                Debug.LogError(this, "Invalid window ID: {0}. Valid range is 1-6", windowId);
                return;
            }

            if (string.IsNullOrEmpty(streamUrl))
            {
                Debug.LogWarning(this, "Stream URL is null or empty for window {0}", windowId);
                return;
            }

            if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.WindowDetails != null)
            {
                // Find the window details for the specified window ID
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                if (windowDetail != null)
                {
                    // Set the stream URL for the window
                    windowDetail.StreamUrl.StringValue = streamUrl;
                    Debug.LogDebug(this, "Set window {0} to stream URL: {1}", windowId, streamUrl);

                    // Update the internal tracking array
                    if (MultiviewWindowStreamUrlFeedbacks != null && windowId <= MultiviewWindowStreamUrlFeedbacks.Count)
                    {
                        MultiviewWindowStreamUrlFeedbacks[(int)windowId - 1]?.FireUpdate();
                    }
                }
                else
                {
                    Debug.LogError(this, "Window detail not found for window ID: {0}", windowId);
                }
            }
            else
            {
                Debug.LogError(this, "DM-NVX-38x multiview window controls not available");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error setting window {0} to stream URL {1}: {2}", windowId, streamUrl, ex.Message);
        }
    }

    /// <summary>
    /// Clear the video source for a specific window.
    /// </summary>
    /// <param name="windowId">Window ID (1-6)</param>
    public void ClearWindowVideoSource(uint windowId)
    {
        var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values.FirstOrDefault(w => w.Number == windowId);
        SetWindowVideoSourceStreamUrl(windowId, string.Empty);
    }

    /// <summary>
    /// Set the label for a specific window.
    /// </summary>
    /// <param name="windowId">Window ID (1-6)</param>
    /// <param name="label">Label to assign to the window</param>
    public void SetWindowLabel(uint windowId, string label)
    {
        try
        {
            if (windowId < 1 || windowId > 6)
            {
                Debug.LogError(this, "Invalid window ID: {0}. Valid range is 1-6", windowId);
                return;
            }

            if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.WindowDetails != null)
            {
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values.FirstOrDefault(w => w.Number == windowId);
                var windowDetailText = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values.FirstOrDefault(w => w.Number == windowId)?.Text.StringValue;

                if (windowDetail != null)
                {
                    // Set the window label
                    windowDetailText = label;
                    Debug.LogDebug(this, "Set window {0} label to: {1}", windowId, label);

                    // Update the internal tracking array
                    if (MultiviewWindowLabelFeedbacks != null && windowId <= MultiviewWindowLabelFeedbacks.Count)
                    {
                        MultiviewWindowLabelFeedbacks[(int)windowId - 1]?.FireUpdate();
                    }
                }
                else
                {
                    Debug.LogError(this, "Window detail not found for window ID: {0}", windowId);
                }
            }
            else
            {
                Debug.LogError(this, "DM-NVX-38x multiview window controls not available");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error setting window {0} label to {1}: {2}", windowId, label, ex.Message);
        }
    }

    /// <summary>
    /// Clears the label of the specified window by setting it to an empty string.
    /// </summary>
    /// <remarks>This method effectively removes any existing label associated with the specified
    /// window.</remarks>
    /// <param name="windowId">The unique identifier of the window whose label is to be cleared.</param>
    public void ClearWindowLabel(uint windowId)
    {
        SetWindowLabel(windowId, string.Empty);
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

        // First set the layout
        if (layout.LayoutIndex >= 0)
        {
            SetWindowLayout((uint)layout.LayoutIndex);
        }

        // Send layout data via messenger
        var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();
        if (mc != null)
        {
            var messenger = new IHasScreensWithLayoutsMessenger($"{Key}-screens", $"/device/{Key}", this);
            mc.AddDeviceMessenger(messenger);
            messenger.SendCurrentLayoutStatus(screenId, layout);
        }
    }

    /// <summary>
    /// Get the current stream URL for a specific window.
    /// </summary>
    /// <param name="windowId">Window ID (1-6)</param>
    /// <returns>Current stream URL or empty string if not found</returns>
    public string GetWindowStreamUrl(uint windowId)
    {
        try
        {
            if (windowId < 1 || windowId > 6)
            {
                Debug.LogError(this, "Invalid window ID: {0}. Valid range is 1-6", windowId);
                return string.Empty;
            }

            if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.WindowDetails != null)
            {
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                if (windowDetail != null)
                {
                    return windowDetail.StreamUrl.StringValue ?? string.Empty;
                }
                else
                {
                    Debug.LogVerbose(this, "Window detail not found for window ID: {0}", windowId);
                }
            }
            else
            {
                Debug.LogVerbose(this, "DM-NVX-38x multiview window controls not available");
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error getting stream URL for window {0}: {1}", windowId, ex.Message);
            return string.Empty;
        }
    }

    /// <summary>
    /// Get the current label for a specific window.
    /// </summary>
    /// <param name="windowId">Window ID (1-6)</param>
    /// <returns>Current window label or empty string if not found</returns>
    public string GetWindowLabel(uint windowId)
    {
        try
        {
            if (windowId < 1 || windowId > 6)
            {
                Debug.LogError(this, "Invalid window ID: {0}. Valid range is 1-6", windowId);
                return string.Empty;
            }

            if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.WindowDetails != null)
            {
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                if (windowDetail != null)
                {
                    return windowDetail.Text.StringValue ?? string.Empty;
                }
                else
                {
                    Debug.LogVerbose(this, "Window detail not found for window ID: {0}", windowId);
                }
            }
            else
            {
                Debug.LogVerbose(this, "DM-NVX-38x multiview window controls not available");
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error getting label for window {0}: {1}", windowId, ex.Message);
            return string.Empty;
        }
    }

    #endregion

    #region Nvx38xLayouts

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

            private Nvx38X _parentDevice;

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
            /// <param name="parentDevice"></param>
            public Nvx38xLayout(string key, string name, int screenIndex, int id, Nvx38X parentDevice)
            {
                Key = key;
                Name = name;
                this.screenIndex = screenIndex;
                Id = id;
                _parentDevice = parentDevice;
            }

            public void Select()
            {
                if (_parentDevice != null)
                {
                    _parentDevice.ApplyLayout((uint)screenIndex, (uint)Id);
                    Debug.LogDebug(_parentDevice, "Applied layout {0} ({1}) to screen {2}", Name, Id, screenIndex);
                }
                else
                {
                    Debug.LogError("Parent device not available for layout selection");
                }
            }
        }
    }

    #endregion

    #region FeedbackCollection Methods

    //Add arrays of collections
    public void AddCollectionsToList(params FeedbackCollection<BoolFeedback>[] newFbs)
    {
        foreach (FeedbackCollection<BoolFeedback> fbCollection in newFbs)
        {
            foreach (var item in newFbs)
            {
                AddCollectionToList(item);
            }
        }
    }
    public void AddCollectionsToList(params FeedbackCollection<IntFeedback>[] newFbs)
    {
        foreach (FeedbackCollection<IntFeedback> fbCollection in newFbs)
        {
            foreach (var item in newFbs)
            {
                AddCollectionToList(item);
            }
        }
    }

    public void AddCollectionsToList(params FeedbackCollection<StringFeedback>[] newFbs)
    {
        foreach (FeedbackCollection<StringFeedback> fbCollection in newFbs)
        {
            foreach (var item in newFbs)
            {
                AddCollectionToList(item);
            }
        }
    }

    //Add Collections
    public void AddCollectionToList(FeedbackCollection<BoolFeedback> newFbs)
    {
        foreach (var f in newFbs)
        {
            if (f == null) continue;

            AddFeedbackToList(f);
        }
    }

    public void AddCollectionToList(FeedbackCollection<IntFeedback> newFbs)
    {
        foreach (var f in newFbs)
        {
            if (f == null) continue;

            AddFeedbackToList(f);
        }
    }

    public void AddCollectionToList(FeedbackCollection<StringFeedback> newFbs)
    {
        foreach (var f in newFbs)
        {
            if (f == null) continue;

            AddFeedbackToList(f);
        }
    }

    //Add Individual Feedbacks
    public void AddFeedbackToList(Feedback newFb)
    {
        if (newFb == null) return;

        if (!Feedbacks.Contains(newFb))
        {
            Feedbacks.Add(newFb);
        }
    }

    #endregion

    #region Event Listener(s)

    void MultiviewWindowLayoutControlsChange(object sender, GenericEventArgs args)
    {
        Debug.LogDebug(this, "WindowLayoutChange event triggerend. EventId = {0}", args.EventId);

        switch(args.EventId)
        {
            case DMOutputEventIds.MultiviewControlsEnabledFeedbackEventId:{ MultiviewEnabledFeedback.FireUpdate(); break; }
            case DMOutputEventIds.MultiviewControlsDisabledFeedbackEventId: { MultiviewDisabledFeedback.FireUpdate(); break; }
            case DMOutputEventIds.MultiviewControlsWindowControlsLayoutFeedbackEventId: { MultiviewLayoutFeedback.FireUpdate(); break; }
            case DMOutputEventIds.MultiviewControlsWindowControlsAudioSourceFeedbackEventId: { MultiviewAudioSourceFeedback.FireUpdate(); break; }
            case DMOutputEventIds.MultiviewControlsWindowControlsStreamUrlFeedbackEventId: { 
                    for (int i = 0; i < MultiviewWindowStreamUrlFeedbacks.Count; i++)
                    {
                        MultiviewWindowStreamUrlFeedbacks[i]?.FireUpdate();
                    }
                    break;
                }
            case DMOutputEventIds.MultiviewControlsWindowControlsTextFeedbackEventId: {
                    for (int i = 0; i < MultiviewWindowLabelFeedbacks.Count; i++)
                    {
                        MultiviewWindowLabelFeedbacks[i]?.FireUpdate();
                    }
                    break;
                }
        }
    }

    #endregion
}