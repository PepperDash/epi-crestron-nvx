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
using NvxEpi.Features.Hdmi.Input;
using NvxEpi.Features.Hdmi.Output;
using NvxEpi.Features.Streams.Usb;

using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using Feedback = PepperDash.Essentials.Core.Feedback;

using HdmiInput = NvxEpi.Features.Hdmi.Input.HdmiInput;

#if SERIES4
using NvxEpi.McMessengers;
#endif

namespace NvxEpi.Devices
{
    public class Nvx36X : 
        NvxBaseDevice, 
        IComPorts, 
        IIROutputPorts,
        IUsbStreamWithHardware, 
        IHdmiInput, 
        IVideowallMode, 
        IRouting, 
        ICec,
        IBasicVolumeWithFeedback
    {
        private IBasicVolumeWithFeedback _audio;
        private IHdmiInput _hdmiInputs;
        private IVideowallMode _hdmiOutput;
        private IUsbStreamWithHardware _usbStream;
        private readonly NvxDeviceProperties _config;

        public Nvx36X(DeviceConfig config, Func<DmNvxBaseClass> getHardware, bool isTransmitter)
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

                _audio = new Nvx36XAudio((DmNvx36x) Hardware, this);
                _usbStream = UsbStream.GetUsbStream(this, _config.Usb);
                _hdmiInputs = new HdmiInput(this);
                _hdmiOutput = new VideowallModeOutput(this);

                Feedbacks.AddRange(new [] { (Feedback)_audio.MuteFeedback, _audio.VolumeLevelFeedback });

                if (_config.EnableAutoRoute)
                    // ReSharper disable once ObjectCreationAsStatement
                    new AutomaticInputRouter(_hdmiInputs);

                AddMcMessengers();

                return result;
            }
            catch (Exception ex)
            {
                Debug.Console(0, this, "Caught an exception in activate:{0}", ex);
                throw;
            }
        }

        public void ClearCurrentUsbRoute()
        {
            _usbStream.ClearCurrentUsbRoute();
        }

        public void MakeUsbRoute(IUsbStreamWithHardware hardware)
        {
            Debug.Console(0, this, "Try Make USB Route for mac : {0}", hardware.UsbLocalId.StringValue);
            var usbStream = _usbStream as UsbStream;
            if (usbStream == null)
            {
                Debug.Console(0, this, "cannot Make USB Route for url : {0} - UsbStream is null", hardware.UsbLocalId.StringValue);
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
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("outputSelector");

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
    }
}