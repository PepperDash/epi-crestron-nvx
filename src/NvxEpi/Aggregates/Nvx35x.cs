using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.Hardware;
using NvxEpi.Entities.HdmiInput;
using NvxEpi.Entities.Streams.Audio;
using NvxEpi.Entities.Streams.Video;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Aggregates
{
    public class Nvx35X : CrestronGenericBridgeableBaseDevice, IComPorts, IIROutputPorts, ICurrentStream,
        IUsbStream, ICurrentVideoInput, ICurrentAudioInput, ICurrentSecondaryAudioStream, IHdmiInput2, IVideowallMode,
        IRouting, ICec
    {
        private readonly ICurrentSecondaryAudioStream _currentSecondaryAudioStream;
        private readonly ICurrentStream _currentVideoStream;
        private readonly Nvx35xHardware _device;
        private readonly IHdmiInput2 _hdmiInput;
        private readonly IUsbStream _usbStream;

        public Nvx35X(DeviceConfig config, DmNvx35x hardware)
            : base(config.Key, config.Name, hardware)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            Hardware = hardware;

            _device = new Nvx35xHardware(config, hardware, Feedbacks, IsOnline);
            _currentVideoStream = new CurrentVideoStream(_device);
            _currentSecondaryAudioStream = new CurrentSecondaryAudioStream(_device);
            _hdmiInput = new HdmiInput2(_device);

            RegisterForOnlineFeedback(hardware, props);
            SetupFeedbacks();
            AddRoutingPorts();
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public StringFeedback CurrentAudioInput
        {
            get { return _device.CurrentAudioInput; }
        }

        public IntFeedback CurrentAudioInputValue
        {
            get { return _device.CurrentAudioInputValue; }
        }

        public IntFeedback CurrentSecondaryAudioStreamId
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamId; }
        }

        public StringFeedback CurrentSecondaryAudioStreamName
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamName; }
        }

        public IntFeedback CurrentStreamId
        {
            get { return _currentVideoStream.CurrentStreamId; }
        }

        public StringFeedback CurrentStreamName
        {
            get { return _currentVideoStream.CurrentStreamName; }
        }

        public StringFeedback CurrentVideoInput
        {
            get { return _device.CurrentVideoInput; }
        }

        public IntFeedback CurrentVideoInputValue
        {
            get { return _device.CurrentVideoInputValue; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public BoolFeedback DisabledByHdcp { get; private set; }

        public new DmNvx35x Hardware { get; private set; }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
        {
            get { return _hdmiInput.HdcpCapability; }
        }

        public IntFeedback HorizontalResolution { get; private set; }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return Hardware.IROutputPorts; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public bool IsRemote
        {
            get { return _usbStream.IsRemote; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _currentSecondaryAudioStream.IsStreamingSecondaryAudio; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _currentVideoStream.IsStreamingVideo; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public int NumberOfComPorts
        {
            get { return Hardware.NumberOfComPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return Hardware.NumberOfIROutputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioAddress; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioStreamStatus; }
        }

        public Cec StreamCec
        {
            get { return Hardware.HdmiOut.StreamCec; }
        }

        public StringFeedback StreamUrl
        {
            get { return _currentVideoStream.StreamUrl; }
        }

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
        {
            get { return _hdmiInput.SyncDetected; }
        }

        public int UsbId
        {
            get { return _usbStream.UsbId; }
        }

        public StringFeedback UsbLocalId
        {
            get { return _usbStream.UsbLocalId; }
        }

        public StringFeedback UsbRemoteId
        {
            get { return _usbStream.UsbRemoteId; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _currentVideoStream.VideoStreamStatus; }
        }

        public IntFeedback VideowallMode { get; private set; }

        DmNvxBaseClass INvxHardware.Hardware
        {
            get { return _device.Hardware; }
        }

        public void ClearSecondaryAudioStream()
        {
            _currentSecondaryAudioStream.ClearSecondaryAudioStream();
        }

        public void ClearStream()
        {
            _currentVideoStream.ClearStream();
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("input selector");

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

        public void SetAudioInput(ushort input)
        {
            ( (IAudioInput) _device ).SetAudioInput(input);
        }

        public void SetAudioToHdmiInput1()
        {
            ( (IAudioInput) _device ).SetAudioToHdmiInput1();
        }

        public void SetAudioToHdmiInput2()
        {
            ( (IAudioInput) _device ).SetAudioToHdmiInput2();
        }

        public void SetAudioToInputAnalog()
        {
            ( (IAudioInput) _device ).SetAudioToInputAnalog();
        }

        public void SetAudioToInputAutomatic()
        {
            ( (IAudioInput) _device ).SetAudioToInputAutomatic();
        }

        public void SetAudioToPrimaryStreamAudio()
        {
            ( (IAudioInput) _device ).SetAudioToPrimaryStreamAudio();
        }

        public void SetAudioToSecondaryStreamAudio()
        {
            ( (IAudioInput) _device ).SetAudioToSecondaryStreamAudio();
        }

        public void SetHdmi1HdcpCapability(int capability)
        {
            _hdmiInput.SetHdmi1HdcpCapability(capability);
        }

        public void SetHdmi1HdcpCapability(eHdcpCapabilityType capability)
        {
            _hdmiInput.SetHdmi1HdcpCapability(capability);
        }

        public void SetHdmi2HdcpCapability(int capability)
        {
            _hdmiInput.SetHdmi2HdcpCapability(capability);
        }

        public void SetHdmi2HdcpCapability(eHdcpCapabilityType capability)
        {
            _hdmiInput.SetHdmi2HdcpCapability(capability);
        }

        public void SetSecondaryAudioAddress(string address)
        {
            _currentSecondaryAudioStream.SetSecondaryAudioAddress(address);
        }

        public void SetStreamUrl(string url)
        {
            _currentVideoStream.SetStreamUrl(url);
        }

        public void SetVideoInput(ushort input)
        {
            ( (IVideoInput) _device ).SetVideoInput(input);
        }

        public void SetVideoToHdmiInput1()
        {
            ( (IVideoInput) _device ).SetVideoToHdmiInput1();
        }

        public void SetVideoToHdmiInput2()
        {
            ( (IVideoInput) _device ).SetVideoToHdmiInput2();
        }

        public void SetVideoToNone()
        {
            ( (IVideoInput) _device ).SetVideoToNone();
        }

        public void SetVideoToStream()
        {
            ( (IVideoInput) _device ).SetVideoToStream();
        }

        public override string ToString()
        {
            return Key;
        }

        private void AddRoutingPorts()
        {
            HdmiInput1Port.AddRoutingPort(this);
            HdmiInput2Port.AddRoutingPort(this);
            HdmiOutput.AddRoutingPort(this);

            if (IsTransmitter)
            {
                StreamOutput.AddRoutingPort(this);
                SecondaryAudioOutput.AddRoutingPort(this);
                AnalogAudioInput.AddRoutingPort(this);
            }
            else
            {
                StreamInput.AddRoutingPort(this);
                SecondaryAudioInput.AddRoutingPort(this);
                AnalogAudioOutput.AddRoutingPort(this);
            }
        }

        private void RegisterForOnlineFeedback(GenericBase hardware, NvxDeviceProperties props)
        {
            hardware.OnlineStatusChange += (device, args) =>
                {
                    if (!args.DeviceOnLine)
                        return;

                    Hardware.Control.Name.StringValue = Name.Replace(' ', '-');

                    if (IsTransmitter)
                        Hardware.SetTxDefaults(props);
                    else
                        Hardware.SetRxDefaults(props);
                };
        }

        private void SetupFeedbacks()
        {
            DisabledByHdcp = HdmiOutputDisabledFeedback.GetFeedback(Hardware);
            HorizontalResolution = HorizontalResolutionFeedback.GetFeedback(Hardware);
            VideowallMode = VideowallModeFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
                {
                    DisabledByHdcp,
                    HorizontalResolution,
                    VideowallMode,
                    Hdmi1HdcpCapabilityFeedback.GetFeedback(Hardware),
                    Hdmi2HdcpCapabilityFeedback.GetFeedback(Hardware)
                });

            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
            DeviceDebug.RegisterForRoutingInputPortFeedback(this);
            DeviceDebug.RegisterForRoutingOutputFeedback(this);
        }
    }
}