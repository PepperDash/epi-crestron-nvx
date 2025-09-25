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

        Screens = new Dictionary<uint, ScreenInfo>(multiviewProps?.Screens ?? new Dictionary<uint, ScreenInfo>());

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
            LayoutNamesFeedbacks.Add(new StringFeedback("LayoutNames-" + screenKey, () => LayoutNames.ContainsKey(screenKey) ? LayoutNames[screenKey] : string.Empty));

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
        _config = NvxDeviceProperties.FromDeviceConfig(config);
        AddPreActivationAction(AddRoutingPorts);
        
        // Initialize collections for basic constructor
        ScreenNamesFeedbacks = new FeedbackCollection<StringFeedback>();
        ScreenEnablesFeedbacks = new FeedbackCollection<BoolFeedback>();
        LayoutNamesFeedbacks = new FeedbackCollection<StringFeedback>();
        LayoutNames = new Dictionary<uint, string>();
        MultiviewWindowStreamUrlFeedbacks = new FeedbackCollection<StringFeedback>();
        MultiviewWindowLabelFeedbacks = new FeedbackCollection<StringFeedback>();
        
        // Initialize empty screens dictionary - no default screens for basic constructor
        Screens = new Dictionary<uint, ScreenInfo>();
        
        Debug.LogInformation(this, "Nvx38X created using basic constructor. Screens initialized as empty dictionary. Count: {0}", Screens?.Count ?? 0);
        
        AddPreActivationAction(InitializeMultiviewFeatures);
        AddPostActivationAction(AddFeedbackCollections);
    }

    #endregion

    #region Generic Methods

    public void ClearCurrentUsbRoute()
    {
        _usbStream?.ClearCurrentUsbRoute();
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
        _audio?.VolumeUp(pressRelease);
    }

    public void VolumeDown(bool pressRelease)
    {
        _audio?.VolumeDown(pressRelease);
    }

    public void MuteToggle()
    {
        _audio?.MuteToggle();
    }

    public void SetVolume(ushort level)
    {
        _audio?.SetVolume(level);
    }

    public void MuteOn()
    {
        _audio?.MuteOn();
    }

    public void MuteOff()
    {
        _audio?.MuteOff();
    }

    #endregion

    #region Interface Properties

    public CrestronCollection<ComPort> ComPorts
    {
        get { return Hardware.ComPorts; }
    }

    public BoolFeedback DisabledByHdcp
    {
        get { return _hdmiOutput?.DisabledByHdcp; }
    }

    public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
    {
        get { return _hdmiInputs?.HdcpCapability; }
    }

    public IntFeedback HorizontalResolution
    {
        get { return _hdmiOutput?.HorizontalResolution; }
    }

    public StringFeedback EdidManufacturer
    {
        get { return _hdmiOutput?.EdidManufacturer; }
    }

    public StringFeedback OutputResolution
    {
        get { return _hdmiOutput?.OutputResolution; }
    }

    public IntFeedback VideoAspectRatioMode
    {
        get { return _hdmiOutput?.VideoAspectRatioMode; }
    }

    public CrestronCollection<IROutputPort> IROutputPorts
    {
        get { return Hardware.IROutputPorts; }
    }

    public bool IsRemote
    {
        get { return _usbStream?.IsRemote ?? false; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> UsbRemoteIds
    {
        get { return _usbStream?.UsbRemoteIds; }
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
        get { return _hdmiInputs?.SyncDetected; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> CurrentResolution
    {
        get { return _hdmiInputs?.CurrentResolution; }
    }

    public ReadOnlyDictionary<uint, IntFeedback> AudioChannels { get { return _hdmiInputs?.AudioChannels; } }

    public ReadOnlyDictionary<uint, StringFeedback> AudioFormat { get { return _hdmiInputs?.AudioFormat; } }

    public ReadOnlyDictionary<uint, StringFeedback> ColorSpace { get { return _hdmiInputs?.ColorSpace; } }

    public ReadOnlyDictionary<uint, StringFeedback> HdrType { get { return _hdmiInputs?.HdrType; } }

    public IntFeedback VideowallMode
    {
        get { return _hdmiOutput?.VideowallMode; }
    }

    public StringFeedback UsbLocalId
    {
        get { return _usbStream?.UsbLocalId; }
    }

    public IntFeedback VolumeLevelFeedback
    {
        get { return _audio?.VolumeLevelFeedback; }
    }

    public BoolFeedback MuteFeedback
    {
        get { return _audio?.MuteFeedback; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> HdcpCapabilityString { get { return _hdmiInputs?.HdcpCapabilityString; } }

    public ReadOnlyDictionary<uint, StringFeedback> HdcpSupport { get { return _hdmiInputs?.HdcpSupport; } }

    #endregion

    #region InitializeMultiviewFeatures

    private void InitializeMultiviewFeatures()
    {
        Debug.LogInformation(this, "InitializeMultiviewFeatures called. Current Screens count: {0}", Screens?.Count ?? 0);
        
        // Initialize multiview window feedbacks for all window IDs
        for (uint windowId = 1; windowId <= 6; windowId++)
        {
            MultiviewWindowStreamUrlFeedbacks.Add(new StringFeedback(
                $"MultiviewWindow{windowId}StreamUrl",
                () => GetWindowStreamUrl(windowId)));

            MultiviewWindowLabelFeedbacks.Add(new StringFeedback(
                $"MultiviewWindow{windowId}Label",
                () => GetWindowLabel(windowId)));
        }

        // Only add default screen if we're being constructed with multiview config
        // Don't add default screen for basic constructor - let it remain empty
        // This prevents the device from being treated as multiview-capable when it shouldn't be
        
        Debug.LogInformation(this, "InitializeMultiviewFeatures completed. Final Screens count: {0}", Screens?.Count ?? 0);
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
                _device = nvx38x;
                
                // Initialize multiview feedbacks if hardware supports it
                if (_device?.MultiviewControlsSetup != null)
                {
                    MultiviewEnabledFeedback = new BoolFeedback("MultiviewEnabledFeedback", () => _device.MultiviewControlsSetup.EnabledFeedback.BoolValue);
                    MultiviewDisabledFeedback = new BoolFeedback("MultiviewDisabledFeedback", () => _device.MultiviewControlsSetup.DisabledFeedback.BoolValue);
                    MultiviewLayoutFeedback = new IntFeedback("MultiviewLayoutFeedback", () => _device.MultiviewControlsSetup.MultiViewWindowControls.LayoutFeedback.UShortValue);
                    MultiviewAudioSourceFeedback = new IntFeedback("MultiviewAudioSourceFeedback", () => _device.MultiviewControlsSetup.MultiViewWindowControls.AudioSourceFeedback.UShortValue);
                    
                    // Register for multiview events
                    _device.MultiviewControlsSetup.MultiViewWindowControls.PropertyChange += MultiviewWindowLayoutControlsChange;
                }
            }

            _usbStream = UsbStream.GetUsbStream(this, _config.Usb);
            _hdmiInputs = new HdmiInput(this);
            _hdmiOutput = new VideowallModeOutput(this);

            if (_audio != null)
                Feedbacks.AddRange(new[] { (Feedback)_audio.MuteFeedback, _audio.VolumeLevelFeedback });

            if (_config.EnableAutoRoute)
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

    /// <summary>
    /// Links the device to the API by mapping its functionality to the specified join map and bridge.
    /// </summary>
    /// <remarks>This method determines whether the device supports multiview functionality based on its
    /// configuration. If multiview screens are present, a specialized multiview join map is used, and the
    /// multiview-specific controls are linked. Otherwise, the device is linked using the standard NVX device
    /// bridge.</remarks>
    /// <param name="trilist">The <see cref="BasicTriList"/> instance used to communicate with the device.</param>
    /// <param name="joinStart">The starting join number for the device's join map.</param>
    /// <param name="joinMapKey">The key used to identify the join map in the bridge.</param>
    /// <param name="bridge">The <see cref="EiscApiAdvanced"/> instance that manages the API bridge. Can be <see langword="null"/> for
    /// certain configurations.</param>
    public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        Debug.LogInformation(this, "LinkToApi called - Device Key: '{0}', JoinStart: {1}, JoinMapKey: '{2}', Bridge: {3}", 
            Key, joinStart, joinMapKey ?? "null", bridge?.GetType().Name ?? "null");
            
        // Check if this device has multiview screens configured
        var hasMultiviewScreens = Screens != null && Screens.Any();
        
        Debug.LogInformation(this, "Multiview screens configured: {0}, Screen count: {1}", 
            hasMultiviewScreens, Screens?.Count ?? 0);
        
        if (hasMultiviewScreens)
        {
            try
            {
                Debug.LogInformation(this, "Linking NVX-38x with multiview capabilities");
                
                // Use the specialized multiview joinMap for NVX-38X
                Debug.LogInformation(this, "Creating Nvx38xMultiviewJoinMap with joinStart: {0}", joinStart);
                
                Nvx38xMultiviewJoinMap multiviewJoinMap;
                try
                {
                    multiviewJoinMap = new Nvx38xMultiviewJoinMap(joinStart);
                    Debug.LogInformation(this, "Successfully created Nvx38xMultiviewJoinMap");
                }
                catch (Exception createEx)
                {
                    Debug.LogError(this, "Error creating Nvx38xMultiviewJoinMap: {0}", createEx.Message);
                    Debug.LogError(this, "Creation Exception details: {0}", createEx.ToString());
                    throw; // Re-throw to trigger fallback
                }

                // Register the multiview joinMap with the bridge FIRST
                Debug.LogInformation(this, "Adding multiview joinMap to bridge for key: {0}", Key);
                bridge?.AddJoinMap(Key, multiviewJoinMap);
                Debug.LogInformation(this, "Successfully registered multiview join map for device key: '{0}'", Key);

                // Link the base NVX functionality using the multiview joinMap
                // Pass null bridge to prevent NvxDeviceBridge from overwriting our multiview joinmap
                var deviceBridge = new NvxDeviceBridge(this);
                deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, null);

                // Link the multiview-specific functionality
                LinkMultiviewControls(trilist, multiviewJoinMap);
                
                Debug.LogInformation(this, "Successfully linked NVX-38x with multiview join map");
            }
            catch (Exception ex)
            {
                Debug.LogError(this, "Error linking multiview joinMap: {0}", ex.Message);
                Debug.LogError(this, "Exception details: {0}", ex.ToString());
                
                // Fall back to standard NVX bridge if multiview linking fails
                Debug.LogInformation(this, "Falling back to standard NVX bridge");
                var fallbackBridge = new NvxDeviceBridge(this);
                fallbackBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
            }
        }
        else
        {
            Debug.LogInformation(this, "Linking NVX-38x with standard capabilities (no multiview screens configured)");
            
            // Use standard NVX device bridge for non-multiview devices
            var deviceBridge = new NvxDeviceBridge(this);
            deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
            
            Debug.LogInformation(this, "Successfully linked NVX-38x with standard join map for device key: '{0}'", Key);
        }
    }

    private void LinkMultiviewControls(BasicTriList trilist, Nvx38xMultiviewJoinMap joinMap)
    {
        try
        {
            Debug.LogDebug(this, "Linking multiview controls for DM-NVX-38x");

            // Multiview Controls: Enter
            trilist.SetBoolSigAction(joinMap.MultiviewEnter.JoinNumber, value =>
            {
                if (value && _device?.MultiviewControlsSetup?.MultiViewWindowControls?.Enter != null)
                {
                    _device.MultiviewControlsSetup.MultiViewWindowControls.Enter.BoolValue = true;
                    Debug.LogDebug(this, "Multiview Enter triggered");
                }
                else if (!value && _device?.MultiviewControlsSetup?.MultiViewWindowControls?.Enter != null)
                {
                    _device.MultiviewControlsSetup.MultiViewWindowControls.Enter.BoolValue = false;
                    Debug.LogDebug(this, "Multiview Enter released");
                }
            });

            // Multiview Controls: Enable/Disable
            trilist.SetBoolSigAction(joinMap.MultiviewEnabled.JoinNumber, value =>
            {
                if (value)
                {
                    _device?.MultiviewControlsSetup?.Enable();
                    Debug.LogDebug(this, "Multiview Enable triggered");
                }
            });

            trilist.SetBoolSigAction(joinMap.MultiviewDisabled.JoinNumber, value =>
            {
                if (value)
                {
                    _device?.MultiviewControlsSetup?.Disable();
                    Debug.LogDebug(this, "Multiview Disable triggered");
                }
            });

            // Feedbacks
            MultiviewEnabledFeedback?.LinkInputSig(trilist.BooleanInput[joinMap.MultiviewEnabled.JoinNumber]);
            MultiviewDisabledFeedback?.LinkInputSig(trilist.BooleanInput[joinMap.MultiviewDisabled.JoinNumber]);

            // Layout Control
            trilist.SetUShortSigAction(joinMap.MultiviewLayout.JoinNumber, layoutValue =>
            {
                if (layoutValue >= 1 && layoutValue <= 18)
                {
                    SetWindowLayout(layoutValue);
                    Debug.LogDebug(this, "Multiview layout set to: {0}", layoutValue);
                }
            });

            MultiviewLayoutFeedback?.LinkInputSig(trilist.UShortInput[joinMap.MultiviewLayoutFeedback.JoinNumber]);

            // Audio Source Control
            trilist.SetUShortSigAction(joinMap.MultiviewAudioSource.JoinNumber, audioSource =>
            {
                if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.AudioSource != null)
                {
                    _device.MultiviewControlsSetup.MultiViewWindowControls.AudioSource.UShortValue = audioSource;
                    Debug.LogDebug(this, "Multiview audio source set to: {0}", audioSource);
                }
            });

            MultiviewAudioSourceFeedback?.LinkInputSig(trilist.UShortInput[joinMap.MultiviewAudioSourceFeedback.JoinNumber]);

            // Window Stream URL Controls and Feedbacks
            for (uint windowId = 1; windowId <= 6; windowId++)
            {
                var windowIndex = windowId;

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

    private void SetWindowLayout(uint layout)
    {
        if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.Layout == null)
            return;

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

    public void SetWindowVideoSourceStreamUrl(uint windowId, string streamUrl)
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
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                if (windowDetail != null)
                {
                    windowDetail.StreamUrl.StringValue = streamUrl ?? string.Empty;
                    Debug.LogDebug(this, "Set window {0} to stream URL: {1}", windowId, streamUrl);

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
                Debug.LogVerbose(this, "DM-NVX-38x multiview window controls not available");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error setting window {0} to stream URL {1}: {2}", windowId, streamUrl, ex.Message);
        }
    }

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
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                if (windowDetail != null)
                {
                    windowDetail.Text.StringValue = label ?? string.Empty;
                    Debug.LogDebug(this, "Set window {0} label to: {1}", windowId, label);

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
                Debug.LogVerbose(this, "DM-NVX-38x multiview window controls not available");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error setting window {0} label to {1}: {2}", windowId, label, ex.Message);
        }
    }

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
            "[ApplyLayout] Applying layout '{0}' ({1}) to screen '{2}' for DM-NVX-38x",
            this, layout.LayoutName, layout.LayoutType, screenId);

        if (layout.LayoutIndex >= 0)
        {
            SetWindowLayout((uint)layoutIndex); // Use layoutIndex instead of layout.LayoutIndex
        }

        var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();
        if (mc != null)
        {
            var messenger = new IHasScreensWithLayoutsMessenger($"{Key}-screens", $"/device/{Key}", this);
            mc.AddDeviceMessenger(messenger);
            messenger.SendCurrentLayoutStatus(screenId, layout);
        }
    }

    public string GetWindowStreamUrl(uint windowId)
    {
        try
        {
            if (windowId < 1 || windowId > 6)
                return string.Empty;

            if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.WindowDetails != null)
            {
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                return windowDetail?.StreamUrl.StringValue ?? string.Empty;
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            Debug.LogError(this, "Error getting stream URL for window {0}: {1}", windowId, ex.Message);
            return string.Empty;
        }
    }

    public string GetWindowLabel(uint windowId)
    {
        try
        {
            if (windowId < 1 || windowId > 6)
                return string.Empty;

            if (_device?.MultiviewControlsSetup?.MultiViewWindowControls?.WindowDetails != null)
            {
                var windowDetail = _device.MultiviewControlsSetup.MultiViewWindowControls.WindowDetails.Values
                    .FirstOrDefault(w => w.Number == windowId);

                return windowDetail?.Text.StringValue ?? string.Empty;
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
                    ItemUpdated?.Invoke(this, EventArgs.Empty);
                }
            }

            private readonly int screenIndex;
            public event EventHandler ItemUpdated;

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

    public void AddCollectionsToList(params FeedbackCollection<BoolFeedback>[] newFbs)
    {
        foreach (var fbCollection in newFbs)
        {
            AddCollectionToList(fbCollection);
        }
    }

    public void AddCollectionsToList(params FeedbackCollection<StringFeedback>[] newFbs)
    {
        foreach (var fbCollection in newFbs)
        {
            AddCollectionToList(fbCollection);
        }
    }

    public void AddCollectionToList(FeedbackCollection<BoolFeedback> newFbs)
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
        Debug.LogDebug(this, "WindowLayoutChange event triggered. EventId = {0}", args.EventId);

        switch(args.EventId)
        {
            case DMOutputEventIds.MultiviewControlsEnabledFeedbackEventId:
                MultiviewEnabledFeedback?.FireUpdate();
                break;
            case DMOutputEventIds.MultiviewControlsDisabledFeedbackEventId:
                MultiviewDisabledFeedback?.FireUpdate();
                break;
            case DMOutputEventIds.MultiviewControlsWindowControlsLayoutFeedbackEventId:
                MultiviewLayoutFeedback?.FireUpdate();
                break;
            case DMOutputEventIds.MultiviewControlsWindowControlsAudioSourceFeedbackEventId:
                MultiviewAudioSourceFeedback?.FireUpdate();
                break;
            case DMOutputEventIds.MultiviewControlsWindowControlsStreamUrlFeedbackEventId:
                for (int i = 0; i < MultiviewWindowStreamUrlFeedbacks?.Count; i++)
                {
                    MultiviewWindowStreamUrlFeedbacks[i]?.FireUpdate();
                }
                break;
            case DMOutputEventIds.MultiviewControlsWindowControlsTextFeedbackEventId:
                for (int i = 0; i < MultiviewWindowLabelFeedbacks?.Count; i++)
                {
                    MultiviewWindowLabelFeedbacks[i]?.FireUpdate();
                }
                break;
        }
    }

    #endregion
}