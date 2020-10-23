﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;

using NvxEpi.DeviceHelpers;
using NvxEpi.Interfaces;
using NvxEpi.Routing;

using PepperDash.Core;
using PepperDash.Essentials.Bridges;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

using Newtonsoft.Json;

namespace NvxEpi
{
    public class NvxDeviceEpi : CrestronGenericBaseDevice, IBridge, INvxDevice, IComPorts, IIROutputPorts
    {
        public RoutingOutputPort RoutingVideoOutput { get; protected set; }
        public RoutingOutputPort RoutingAudioOutput { get; protected set; }

        public static readonly string DefaultRouterKey = "Default";

        public string ParentRouterKey { get; private set; }

        protected DmNvxBaseClass _device;
        protected NvxDevicePropertiesConfig _propsConfig;

        protected ISwitcher _videoSwitcher;
        protected ISwitcher _audioSwitcher;
        protected ISwitcher _videoInputSwitcher;
        protected ISwitcher _audioInputSwitcher;
        protected NvxVideoWallHelper _videoWall;

        protected List<INvxHdmiInputHelper> _inputs;

        protected IEnumerable<INvxDevice> _devices;

        public IEnumerable<INvxDevice> Devices
        {
            get { return _devices; }
        }
        public IEnumerable<INvxDevice> Transmitters
        {
            get { return _devices.Where(x => x.IsTransmitter); }
        }
        public IEnumerable<INvxDevice> Receivers
        {
            get { return _devices.Where(x => !x.IsTransmitter); }
        }

        public int VirtualDevice { get; protected set; }

        protected bool _isTransmitter;
        public bool IsTransmitter
        {
            get 
            {
                return _isTransmitter;
            }
        }

        public string LocalUsbId
        {
            get { return _device.UsbInput.LocalDeviceIdFeedback.StringValue; }
        }

        public string RemoteUsbId
        {
            get 
            { 
                return _device.UsbInput.RemoteDeviceIdFeedback.StringValue; 
            }
            set { _device.UsbInput.RemoteDeviceId.StringValue = value; }
        }

        public void Pair()
        {
            _device.UsbInput.Pair();
        }

        public void RemovePairing()
        {
            _device.UsbInput.RemovePairing();
        }

        public Feedback StreamStartedFb { get; protected set; }
        public bool StreamStarted
        {
            get
            {
                return _device.Control.StartFeedback.BoolValue;
            }
        }

        public Feedback HdmiInput1SyncDetectedFb
        {
            get { return _inputs == null ? null : _inputs[0].SyncDetectedFb; }
            protected set
            {
                if (_inputs == null) return;
                _inputs[0].SyncDetectedFb = value;
            }
        }
        public bool HdmiInput1SyncDetected
        {
            get
            {
                if (_inputs == null) return false;

                return _inputs[0] != null && (_inputs[0].SyncDetected);
            }
        }

        public Feedback HdmiInput2SyncDetectedFb
        {
            get
            {
                if (_inputs == null) return null;

                return _inputs.Count < 2 ? null : _inputs[1].SyncDetectedFb;
            }
            protected set
            {
                if (_inputs == null) return;
                if (_inputs.Count < 2) return;
                _inputs[1].SyncDetectedFb = value;
            }
        }
        public bool HdmiInput2SyncDetected
        {
            get
            {
                if (_inputs == null) return false;
                if (_inputs.Count < 2) return false;

                return _inputs[1] != null && _inputs[1].SyncDetected;
            }
        }

        public Feedback VideoSourceFb { get { return _videoSwitcher.Feedback; } }
        public int VideoSource
        {
            get { return _videoSwitcher.Source; }
            set { _videoSwitcher.Source = value; }
        }

        public Feedback AudioSourceFb { get { return _audioSwitcher.Feedback; } }
        public int AudioSource
        {
            get { return _audioSwitcher.Source; }
            set { _audioSwitcher.Source = value; }
        }

        public Feedback VideoInputSourceFb { get { return _videoInputSwitcher.Feedback; } }
        public int VideoInputSource
        {
            get { return _videoInputSwitcher.Source; }
            set { _videoInputSwitcher.Source = value; }
        }

        public Feedback AudioInputSourceFb { get { return _audioInputSwitcher.Feedback; } }
        public int AudioInputSource
        {
            get { return _audioInputSwitcher.Source; }
            set { _audioInputSwitcher.Source = value; }
        }

        public Feedback DeviceModeFb { get; protected set; }
        public int DeviceMode
        {
            get 
            {
                var result = eDeviceMode.Receiver;
                if (_device is DmNvxE3x)
                {
                    result = eDeviceMode.Transmitter;
                }
                else
                {
                    result = _device.Control.DeviceMode;
                }
                Debug.Console(2, this, "Device Mode = {0}", _device.Control.DeviceMode);
                return (int)_device.Control.DeviceMode; 
            }
        }

        public Feedback HdmiInput1HdmiCapabilityFb
        {
            get
            {
                return _inputs == null ? null : _inputs[0].HdmiCapabilityFb;
            }
        }
        public int HdmiInput1HdmiCapability
        {
            get
            {
                return _inputs == null ? 0 : (_inputs[0].HdmiCapability);
            }
            set
            {
                if (_inputs == null) return;

                if (_inputs[0] == null) return;

                _inputs[0].HdmiCapability = value;
            }
        }

        public Feedback HdmiInput1SupportedLevelFb
        {
            get { return _inputs == null ? null : _inputs[0].HdmiSupportedLevelFb; }
        }
        public int HdmiInput1SupportedLevel
        {
            get
            {
                if (_inputs == null) return default(int);

                return _inputs[0] == null ? default(int) : (_inputs[0].HdmiSupportedLevel);
            }
        }

        public Feedback HdmiInput2HdmiCapabilityFb
        {
            get
            {
                if (_inputs == null) return null;

                return _inputs.Count < 2 ? null : _inputs[1].HdmiCapabilityFb;
            }
        }
        public int HdmiInput2HdmiCapability
        {
            get
            {
                if (_inputs == null) return default(int);
                if (_inputs.Count < 2) return default(int);

                return _inputs[1] == null ? default(int) : (_inputs[1].HdmiCapability);
            }
            set
            {
                if (_inputs == null) return;
                if (_inputs.Count < 2) return;

                if (_inputs[1] == null) return;
                _inputs[1].HdmiCapability = value;
            }
        }

        public Feedback HdmiInput2SupportedLevelFb
        {
            get
            {
                if (_inputs == null) return null;

                return _inputs.Count < 2 ? null : _inputs[1].HdmiSupportedLevelFb;
            }
        }
        public int HdmiInput2SupportedLevel
        {
            get
            {
                if (_inputs == null) return default(int);
                if (_inputs.Count < 2) return default(int);

                return _inputs[1] == null ? default(int) : (_inputs[1].HdmiSupportedLevel);
            }
        }

        public Feedback OutputResolutionFb { get; protected set; }
        public int OutputResolution
        {
            get
            {
                return _device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue;
            }
        }

        public Feedback VideoWallModeFb { get; protected set; }
        public int VideoWallMode
        {
            get
            {
                return  _device.HdmiOut == null || _device.HdmiOut.VideoWallModeFeedback == null ? 0 : 
                    _device.HdmiOut.VideoWallModeFeedback.UShortValue;
            }
        }

        public Feedback DeviceNameFb { get; protected set; }
        public string DeviceName
        {
            get 
            {
                return _device.Control.NameFeedback.StringValue; 
            }
            set { _device.Control.Name.StringValue = value; }
        }

        public Feedback DeviceStatusFb { get; protected set; }
        public string DeviceStatus
        {
            get 
            {
                return _device.Control.StatusTextFeedback.StringValue; 
            }
        }

        public Feedback StreamUrlFb { get; protected set; }
        public string StreamUrl
        {
            get 
            { 
                return _device.Control.ServerUrlFeedback.StringValue; 
            }
            set { _device.Control.ServerUrl.StringValue = value; }
        }

        public Feedback MulticastVideoAddressFb { get; protected set; }
        public string MulticastVideoAddress
        {
            get 
            {
               return _device.Control.MulticastAddressFeedback.StringValue; 
            }
        }

        public Feedback MulticastAudioAddressFb { get; protected set; }
        public string MulticastAudioAddress
        {
            get
            {
                return _device.SecondaryAudio == null ? String.Empty :
                    _device.SecondaryAudio.MulticastAddressFeedback.StringValue;
            }
        }

        public Feedback CurrentlyRoutedVideoSourceFb { get; protected set; }
        public string CurrentlyRoutedVideoSource
        {
            get
            {
                return _videoSwitcher.CurrentlyRouted;
            }
        }

        public Feedback CurrentlyRoutedAudioSourceFb { get; protected set; }
        public string CurrentlyRoutedAudioSource
        {
            get
            {
                return _audioSwitcher.CurrentlyRouted;
            }
        }

        public Feedback OutputDisabledByHdcpFb { get; protected set; }

        public Feedback IsOnlineFb { get { return IsOnline; } }

        public NvxDeviceEpi(string key, string name, DmNvxBaseClass device, NvxDevicePropertiesConfig config, 
            ISwitcher videoSwitcher, ISwitcher audioSwitcher, ISwitcher videoInputSwitcher, ISwitcher audioInputSwitcher,
            NvxVideoWallHelper videoWallHelper, List<INvxHdmiInputHelper> inputs)
            : base(key, name, device)
        {
            _device = device;
            _propsConfig = config;

            VirtualDevice = config.VirtualDevice;
            ParentRouterKey = config.ParentDeviceKey ?? DefaultRouterKey;

            _videoSwitcher = videoSwitcher;
            _audioSwitcher = audioSwitcher;

            _videoInputSwitcher = videoInputSwitcher;
            _audioInputSwitcher = audioInputSwitcher;
            _videoWall = videoWallHelper;

            _inputs = inputs;

            var videoOutputName = string.Format("{0}-VideoOutput", Key);
            RoutingVideoOutput = new RoutingOutputPort(videoOutputName, eRoutingSignalType.Video, eRoutingPortConnectionType.Streaming, null, this);

            var audioOutputName = string.Format("{0}-AudioOutput", Key);
            RoutingAudioOutput = new RoutingOutputPort(audioOutputName, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, null, this);

            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>() { RoutingVideoOutput, RoutingAudioOutput };

            if (!String.IsNullOrEmpty(_propsConfig.UsbMode))
            {
                var usbMode =
                    (DmNvxUsbInput.eUsbMode)Enum.Parse(typeof(DmNvxUsbInput.eUsbMode), config.UsbMode, true);

                _device.UsbInput.Mode = usbMode;
            }

            AddPostActivationAction(() => 
                {
                    _devices = DeviceManager
                        .AllDevices
                        .OfType<INvxDevice>()
                        .Where(x => x.ParentRouterKey.Equals(ParentRouterKey, StringComparison.OrdinalIgnoreCase))
                        .Where(x => !x.Key.Equals(Key));

                    _videoSwitcher.SetInputs(Transmitters);
                    _audioSwitcher.SetInputs(Transmitters);
                    _videoInputSwitcher.SetInputs(Transmitters);
                    _audioInputSwitcher.SetInputs(Transmitters);
                });

            AddPostActivationAction(() => _devices.ToList().ForEach(dev => Debug.Console(1, this, "Found device : {0} | IsTransmitter:{1}", dev.Key, dev.IsTransmitter)));

            AddPostActivationAction(SetupRoutingPorts);
        }

        public override bool CustomActivate()
        {
            DeviceNameFb = FeedbackFactory.GetFeedback(() => (String.IsNullOrEmpty(_propsConfig.FriendlyName)) ? DeviceName : _propsConfig.FriendlyName);
            DeviceModeFb = FeedbackFactory.GetFeedback(() => DeviceMode);
            StreamStartedFb = FeedbackFactory.GetFeedback(() => StreamStarted);
            OutputResolutionFb = FeedbackFactory.GetFeedback(() => OutputResolution);
            VideoWallModeFb = FeedbackFactory.GetFeedback(() => VideoWallMode);
            MulticastVideoAddressFb = FeedbackFactory.GetFeedback(() => MulticastVideoAddress);
            MulticastAudioAddressFb = FeedbackFactory.GetFeedback(() => MulticastAudioAddress);
            CurrentlyRoutedVideoSourceFb = FeedbackFactory.GetFeedback(() => CurrentlyRoutedVideoSource);
            CurrentlyRoutedAudioSourceFb = FeedbackFactory.GetFeedback(() => CurrentlyRoutedAudioSource);
            StreamUrlFb = FeedbackFactory.GetFeedback(() => StreamUrl);
            DeviceStatusFb = FeedbackFactory.GetFeedback(() => DeviceStatus);

            OutputDisabledByHdcpFb = new BoolFeedback(() =>
                {
                    if (IsTransmitter) return false;
                    return _device.HdmiOut.DisabledByHdcpFeedback.BoolValue;
                });

            AddToFeedbackList(DeviceNameFb, DeviceModeFb, StreamUrlFb, VideoWallModeFb, MulticastVideoAddressFb, MulticastAudioAddressFb);

            var result = base.CustomActivate();
            
            SubscribeToEvents();
            SetDefaults();

            return result;
        }

        public void LinkToApi(Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist, uint joinStart, string joinMapKey)
        {
            this.LinkToApiExt(trilist, joinStart, joinMapKey);
        }

        protected void SubscribeToEvents()
        {
            _device.BaseEvent += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMInputEventIds.DeviceModeFeedbackEventId:
                            Debug.Console(2, this, "Device Mode Updated: {0}", _device.Control.DeviceModeFeedback);
                            if (DeviceModeFb != null) DeviceModeFb.FireUpdate();
                            break;
                        case DMInputEventIds.StreamUriFeedbackEventId:
                            if (StreamUrlFb != null) StreamUrlFb.FireUpdate();
                            break;
                        case DMInputEventIds.ServerUrlEventId:
                            if (StreamUrlFb != null) StreamUrlFb.FireUpdate();
                            break;
                        case DMInputEventIds.StartEventId:
                            if (StreamStartedFb != null) StreamStartedFb.FireUpdate();
                            break;
                        case DMInputEventIds.StopEventId:
                            if (StreamStartedFb != null) StreamStartedFb.FireUpdate();
                            break;
                        case DMInputEventIds.MulticastAddressEventId:
                            break;
                        case DMInputEventIds.NameFeedbackEventId:
                            if (DeviceNameFb != null) DeviceNameFb.FireUpdate();
                            break;
                        case DMInputEventIds.StatusTextEventId:
                            if (DeviceStatusFb != null) DeviceStatusFb.FireUpdate();
                            break;
                        case DMInputEventIds.StatusEventId:
                            if (DeviceStatusFb != null) DeviceStatusFb.FireUpdate();
                            break;
                        default:
                            //Debug.Console(2, this, "Base Event Unhandled DM EventId {0}", args.EventId);
                            break;
                    };
                };

            if (_device.HdmiOut != null)
            {
                _device.HdmiOut.StreamChange += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMOutputEventIds.ResolutionEventId:
                            if (OutputResolutionFb != null) OutputResolutionFb.FireUpdate();
                            break;
                        case DMOutputEventIds.DisabledByHdcpEventId:
                            if (OutputDisabledByHdcpFb != null) OutputDisabledByHdcpFb.FireUpdate();
                            break;
                        default:
                            //Debug.Console(2, this, "Stream Change Unhandled DM EventId {0}", args.EventId);
                            break;
                    }
                };
            }

            _videoSwitcher.RouteUpdated += (sender, args) =>
                {
                    if (MulticastVideoAddressFb != null) MulticastVideoAddressFb.FireUpdate();
                    if (CurrentlyRoutedVideoSourceFb != null) CurrentlyRoutedVideoSourceFb.FireUpdate();  
                };

            _audioSwitcher.RouteUpdated += (sender, args) =>
                {
                    if (MulticastAudioAddressFb != null) MulticastAudioAddressFb.FireUpdate();
                    if (CurrentlyRoutedAudioSourceFb != null) CurrentlyRoutedAudioSourceFb.FireUpdate();
                };
        }

        protected void SetDefaults()
        {
            if (String.IsNullOrEmpty(_propsConfig.Mode))
            {
                var ex = string.Format("The device mode MUST be defined in the config file: {0}", Key);
                Debug.ConsoleWithLog(0, ex);

                throw new Exception(ex);
            }

            var mode = _propsConfig.Mode;
            var audioBreakaway = _propsConfig.EnableAudioBreakaway;

            if (mode.Equals("tx", StringComparison.OrdinalIgnoreCase))
            {
                _isTransmitter = true;
                _device.Control.DeviceMode = eDeviceMode.Transmitter;               
                _device.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
                _device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;

                if (!String.IsNullOrEmpty(_propsConfig.MulticastVideoAddress)) 
                {
                    _device.Control.MulticastAddress.StringValue = _propsConfig.MulticastVideoAddress;
                }

                if (!String.IsNullOrEmpty(_propsConfig.MulticastAudioAddress))
                {
                    _device.SecondaryAudio.MulticastAddress.StringValue = _propsConfig.MulticastAudioAddress;
                }
            }
            else
            {
                _device.Control.DeviceMode = eDeviceMode.Receiver;
                _device.Control.VideoSource = eSfpVideoSourceTypes.Stream;
                _device.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;

                if (audioBreakaway)
                {
                    _device.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
                    _device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                }
            }
    
            _device.SecondaryAudio.EnableAutomaticInitiation();
        }

        protected void SetupRoutingPorts()
        {
            if (_device.HdmiIn != null)
            {
                for (uint x = 0; x < _device.HdmiIn.Count; x++)
                {
                    var inputNumber = x + 1;

                    InputPorts.Add(new RoutingInputPort(
                        string.Format("{0}-Hdmi{1}", Key, inputNumber),
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        new NvxInputSourceSelector() {HdmiInput = (int) inputNumber},
                        this));
                }
            }

            if (_isTransmitter) return;
            foreach (var device in Transmitters)
            {
                if (device.Key.Equals(Key, StringComparison.InvariantCultureIgnoreCase)) continue;

                var videoRoutingInput = new RoutingInputPort(
                        string.Format("{0}-VideoStreamId:{1}", Key, device.VirtualDevice),
                        eRoutingSignalType.Video,
                        eRoutingPortConnectionType.Streaming,
                        new NvxInputSourceSelector() { Device = device },
                        this);

                var audioRoutingInput = new RoutingInputPort(
                        string.Format("{0}-VideoStreamId:{1}", Key, device.VirtualDevice),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        new NvxInputSourceSelector() { Device = device },
                        this);

                InputPorts.Add(videoRoutingInput);
                InputPorts.Add(audioRoutingInput);

                TieLineCollection.Default.Add(new TieLine(device.RoutingVideoOutput, videoRoutingInput));
                TieLineCollection.Default.Add(new TieLine(device.RoutingAudioOutput, audioRoutingInput));
            }
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return _device.ComPorts; }
        }

        public int NumberOfComPorts
        {
            get { return _device.NumberOfComPorts; }
        }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return _device.IROutputPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return _device.NumberOfIROutputPorts; }
        }

        #region IRouting Members

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var input = inputSelector as NvxInputSourceSelector;
            if (input == null) return;

            switch (signalType)
            {
                case eRoutingSignalType.AudioVideo:
                    if (input.Device == null)
                    {
                        _videoInputSwitcher.Source = input.HdmiInput;
                        _audioInputSwitcher.Source = input.HdmiInput;
                    }
                    else
                    {
                        VideoSource = input.Device.VirtualDevice;
                        AudioSource = input.Device.VirtualDevice;
                    }

                    break;
                case eRoutingSignalType.Video:
                    if (input.Device == null)
                    {
                        _videoInputSwitcher.Source = input.HdmiInput;
                    }
                    else
                    {
                        VideoSource = input.Device.VirtualDevice;
                    }

                    break;
                case eRoutingSignalType.Audio:
                    if (input.Device == null)
                    {
                        _audioInputSwitcher.Source = input.HdmiInput;
                    }
                    else
                    {
                        AudioSource = input.Device.VirtualDevice;
                    }

                    break;
            }
        }

        #endregion

        #region IRoutingInputs Members

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; protected set; }

        #endregion

        #region IRoutingOutputs Members

        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; protected set; }

        #endregion
    }
}

