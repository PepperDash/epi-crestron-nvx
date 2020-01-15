using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using EssentialsExtensions;
using EssentialsExtensions.Attributes;
using NvxEpi.DeviceHelpers;
using NvxEpi.Interfaces;
using PepperDash.Core;
using PepperDash.Essentials.Bridges;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Devices;

namespace NvxEpi
{
    public class NvxDeviceEpi : CrestronGenericBaseDevice, IBridge, INvxDevice, IComPorts, IIROutputPorts
    {
        protected DmNvxBaseClass _device;
        protected DeviceConfig _config;

        protected ISwitcher _videoSwitcher;
        protected ISwitcher _audioSwitcher;
        protected ISwitcher _videoInputSwitcher;
        protected ISwitcher _audioInputSwitcher;

        protected List<INvxHdmiInputHelper> _inputs;

        protected static List<INvxDevice> _devices = new List<INvxDevice>();

        public static IEnumerable<INvxDevice> Devices
        {
            get { return _devices; }
        }
        public static IEnumerable<INvxDevice> Transmitters
        {
            get { return _devices.Where(x => x.IsTransmitter); }
        }
        public static IEnumerable<INvxDevice> Receivers
        {
            get { return _devices.Where(x => x.IsReceiver); }
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

        protected bool _isReceiver;
        public bool IsReceiver
        {
            get 
            {
                return _isReceiver;
            }
        }

        [Feedback(JoinNumber = 2)]
        public Feedback StreamStartedFb { get; protected set; }
        public bool StreamStarted
        {
            get
            {
                return _device.Control.StartFeedback.BoolValue;
            }
        }

        [Feedback(JoinNumber = 3)]
        public Feedback HdmiInput1SyncDetectedFb
        {
            get { return _inputs[0].SyncDetectedFb; }
            protected set { _inputs[0].SyncDetectedFb = value; }
        }
        public bool HdmiInput1SyncDetected
        {
            get
            {
                if (_inputs[0] == null) return false;
                return (_inputs[0].SyncDetected);
            }
        }

        [Feedback(JoinNumber = 4)]
        public Feedback HdmiInput2SyncDetectedFb
        {
            get { return _inputs[1].SyncDetectedFb; }
            protected set { _inputs[1].SyncDetectedFb = value; }
        }
        public bool HdmiInput2SyncDetected
        {
            get
            {
                if (_inputs[1] == null) return false;
                return (_inputs[1].SyncDetected);
            }
        }

        public int VideoSource
        {
            get { return _videoSwitcher.Source; }
            set { _videoSwitcher.Source = value; }
        }

        public int AudioSource
        {
            get { return _audioSwitcher.Source; }
            set { _audioSwitcher.Source = value; }
        }

        public int VideoInputSource
        {
            get { return _videoInputSwitcher.Source; }
            set { _videoInputSwitcher.Source = value; }
        }

        public int AudioInputSource
        {
            get { return _audioInputSwitcher.Source; }
            set { _audioInputSwitcher.Source = value; }
        }

        [Feedback(JoinNumber = 5)]
        public Feedback DeviceModeFb { get; protected set; }
        public int DeviceMode
        {
            get 
            {
                var result = eDeviceMode.Receiver;
                if (_device.GetType().GetCType() == typeof(DmNvxE30).GetCType())
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

        [Feedback(JoinNumber = 6)]
        public Feedback HdmiInput1HdmiCapabilityFb
        {
            get 
            { 
                return _inputs[0].HdmiCapabilityFb;
            }
            protected set 
            { 
                _inputs[0].HdmiCapabilityFb = value; 
            }
        }
        public int HdmiInput1HdmiCapability
        {
            get
            {
                if (_inputs[0] == null) return default(int);
                return (_inputs[0].HdmiCapability);
            }
            set
            {
                if (_inputs[0] == null) return;
                _inputs[0].HdmiCapability = value;
            }
        }

        [Feedback(JoinNumber = 7)]
        public Feedback HdmiInput1SupportedLevelFb
        {
            get { return _inputs[0].HdmiSupportedLevelFb; }
            protected set { _inputs[0].HdmiSupportedLevelFb = value; }
        }
        public int HdmiInput1SupportedLevel
        {
            get
            {
                if (_inputs[0] == null) return default(int);
                return (_inputs[0].HdmiSupportedLevel);
            }
        }

        [Feedback(JoinNumber = 8)]
        public Feedback HdmiInput2HdmiCapabilityFb
        {
            get { return _inputs[1].HdmiCapabilityFb; }
            protected set { _inputs[1].HdmiCapabilityFb = value; }
        }
        public int HdmiInput2HdmiCapability
        {
            get
            {
                if (_inputs[1] == null) return default(int);
                return (_inputs[1].HdmiCapability);
            }
        }

        [Feedback(JoinNumber = 9)]
        public Feedback HdmiInput2SupportedLevelFb
        {
            get { return _inputs[1].HdmiSupportedLevelFb; }
            protected set { _inputs[1].HdmiSupportedLevelFb = value; }
        }
        public int HdmiInput2SupportedLevel
        {
            get
            {
                if (_inputs[1] == null) return default(int);
                return (_inputs[1].HdmiSupportedLevel);
            }
        }

        [Feedback(JoinNumber = 10)]
        public Feedback OutputResolutionFb { get; protected set; }
        public int OutputResolution
        {
            get
            {
                return _device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue;
            }
        }

        [Feedback(JoinNumber = 1)]
        public Feedback DeviceNameFb { get; protected set; }
        public string DeviceName
        {
            get 
            { 
                return _device.Control.NameFeedback.StringValue; 
            }
            set { _device.Control.Name.StringValue = value; }
        }

        [Feedback(JoinNumber = 2)]
        public Feedback DeviceStatusFb { get; protected set; }
        public string DeviceStatus
        {
            get 
            {
                return _device.Control.StatusTextFeedback.StringValue; 
            }
        }

        [Feedback(JoinNumber = 3)]
        public Feedback StreamUrlFb { get; protected set; }
        public string StreamUrl
        {
            get 
            { 
                return _device.Control.ServerUrlFeedback.StringValue; 
            }
        }

        [Feedback(JoinNumber = 4)]
        public Feedback MulticastVideoAddressFb { get; protected set; }
        public string MulticastVideoAddress
        {
            get 
            {
               return _device.Control.MulticastAddressFeedback.StringValue; 
            }
        }

        [Feedback(JoinNumber = 5)]
        public Feedback MulticastAudioAddressFb { get; protected set; }
        public string MulticastAudioAddress
        {
            get
            {
                string result = string.Empty;
                if (_audioSwitcher is NvxReceiveAudioSwitcher)
                {    
                    result = _device.SecondaryAudio.ReceiveMulticastAddressFeedback.StringValue;
                }
                else
                {
                    result = _device.SecondaryAudio.MulticastAddressFeedback.StringValue;
                }
                return result;
            }
        }

        [Feedback(JoinNumber = 6)]
        public Feedback CurrentlyRoutedVideoSourceFb { get; protected set; }
        public string CurrentlyRoutedVideoSource
        {
            get
            {
                return _videoSwitcher.CurrentlyRouted;
            }
        }

        [Feedback(JoinNumber = 7)]
        public Feedback CurrentlyRoutedAudioSourceFb { get; protected set; }
        public string CurrentlyRoutedAudioSource
        {
            get
            {
                return _audioSwitcher.CurrentlyRouted;
            }
        }

        public NvxDeviceEpi(DeviceConfig config, DmNvxBaseClass device)
            : base(config.Key, config.Name, device)
        {
            _device = device;
            _config = config;

            VirtualDevice = config.Properties.Value<int>("virtualDevice");

            _videoSwitcher = new NvxVideoSwitcher(config, _device).BuildFeedback();
            _audioSwitcher = new NvxAudioSwitcher(config, _device).BuildFeedback();

            _videoInputSwitcher = new NvxVideoInputHandler(config, _device).BuildFeedback();
            _audioInputSwitcher = new NvxAudioInputHandler(config, _device).BuildFeedback();

            _inputs = new List<INvxHdmiInputHelper>();
            foreach (var input in _device.HdmiIn)
            {
                _inputs.Add(new NvxHdmiInputHelper(config, input, device));
            }

            AddPreActivationAction(() => _devices.Add(this));
        }

        public override bool CustomActivate()
        {
            var result = base.CustomActivate();

            AddToFeedbackList(StreamUrlFb, MulticastVideoAddressFb, MulticastAudioAddressFb);

            SubscribeToEvents();
            SetDefaults();

            return result;
        }

        public virtual void LinkToApi(Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist, uint joinStart, string joinMapKey)
        {
            IsOnline.LinkInputSig(trilist.BooleanInput[1]);

            this.LinkFeedback(trilist, joinStart, joinMapKey);
            var t = this.GetType().GetCType();

            var fields = t
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var m = t.GetField(field.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(this) as IDynamicFeedback;

                if (m == null) continue;

                m.LinkFeedback(trilist, joinStart, joinMapKey);
            }
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

            _device.HdmiOut.StreamChange += (sender, args) =>
                {
                    switch (args.EventId)
                    {
                        case DMOutputEventIds.ResolutionEventId:
                            if (OutputResolutionFb != null) OutputResolutionFb.FireUpdate();
                            break;
                        default:
                            //Debug.Console(2, this, "Stream Change Unhandled DM EventId {0}", args.EventId);
                            break;
                    }
                };

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
            var mode = _config.Properties.Value<string>("mode");
            var audioBreakaway = _config.Properties.Value<bool>("enableAudioBreakaway");

            if (mode == null)
            {
                var ex = string.Format("The device mode MUST be defined in the config file: {0}", Key);
                Debug.ConsoleWithLog(0, ex);

                throw new Exception(ex);
            }

            if (mode.Equals("rx", StringComparison.InvariantCultureIgnoreCase))
            {
                _isReceiver = true;
                _device.Control.DeviceMode = eDeviceMode.Receiver;
                _device.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;

                if (audioBreakaway) _device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                
                _videoInputSwitcher.Source = 3;
                _audioInputSwitcher.Source = 5;
            }

            if (mode.Equals("tx", StringComparison.InvariantCultureIgnoreCase))
            {
                _isTransmitter = true;
                _device.Control.DeviceMode = eDeviceMode.Transmitter;               
                _device.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
                _device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;

                var multicastVideo = _config.Properties.Value<string>("multicastVideoAddress");
                
                if (multicastVideo != null) 
                {
                    _device.Control.MulticastAddress.StringValue = multicastVideo;
                }

                var multicastAudio = _config.Properties.Value<string>("multicastAudioAddress");

                if (multicastAudio != null)
                {
                    _device.SecondaryAudio.MulticastAddress.StringValue = multicastAudio;
                }
            }

            _device.Control.EnableAutomaticInitiation();
            _device.SecondaryAudio.EnableAutomaticInitiation();
        }

        protected static DmNvxBaseClass GetNvxDevice(DeviceConfig config)
        {
            var name = config.Properties.Value<string>("deviceName");
            var model = config.Properties.Value<string>("model");
            var ipid = config.Properties.Value<string>("ipid");

            try
            {
                var nvxDeviceType = typeof(DmNvxBaseClass)
                    .GetCType()
                    .Assembly
                    .GetTypes()
                    .FirstOrDefault(x => x.Name.Equals(model, StringComparison.OrdinalIgnoreCase));

                if (nvxDeviceType == null) throw new NullReferenceException();
                
                var newDevice = nvxDeviceType
                    .GetConstructor(new CType[] { typeof(ushort).GetCType(), typeof(CrestronControlSystem) })
                    .Invoke(new object[] { Convert.ToUInt16(ipid, 16), Global.ControlSystem });

                var nvxDevice = newDevice as DmNvxBaseClass;
                if (nvxDevice == null) throw new NullReferenceException("Could not find the base nvx type");

                if (name != null) nvxDevice.Control.Name.StringValue = name.Replace(" ", string.Empty);
                else nvxDevice.Control.Name.StringValue = config.Name.Replace(" ", string.Empty);

                return nvxDevice;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void LoadPlugin()
        {
            DeviceFactory.AddFactoryForType("NvxDevice", NvxDeviceEpi.Build);
        }

        public static NvxDeviceEpi Build(DeviceConfig config)
        {
            var device = NvxDeviceEpi.GetNvxDevice(config);
            return new NvxDeviceEpi(config, device).BuildFeedback();
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
    }
}

