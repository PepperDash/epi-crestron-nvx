using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.Hardware;
using NvxEpi.Entities.Routing;
using NvxEpi.Entities.Streams;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Aggregates
{
    public class Nvx35X : CrestronGenericBridgeableBaseDevice, IComPorts, IIROutputPorts, ICurrentStream, IUsbStream,
        ICurrentVideoInput, ICurrentAudioInput, ICurrentSecondaryAudioStream, IHdmiInput, IVideowallMode, IRouting
    {
        private readonly Nvx35xHardware _device;
        private readonly ICurrentStream _currentVideoStream;
        private readonly ICurrentSecondaryAudioStream _currentSecondaryAudioStream;
        private readonly IUsbStream _usbStream;
        private readonly IRouting _router;

        private readonly Dictionary<uint, IntFeedback> _hdcpCapability = 
            new Dictionary<uint, IntFeedback>();

        private readonly Dictionary<uint, BoolFeedback> _syncDetected = 
            new Dictionary<uint, BoolFeedback>();

        public Nvx35X(DeviceConfig config, DmNvx35x hardware)
            : base(config.Key, config.Name, hardware)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            Hardware = hardware;

            _device = new Nvx35xHardware(config, hardware, Feedbacks, IsOnline);
            _currentVideoStream = new CurrentVideoStream(new VideoStream(_device));
            _currentSecondaryAudioStream = new CurrentSecondaryAudioStream(new SecondaryAudioStream(_device));
            _router = new NvxDeviceRouter(_device);

            RegisterForOnlineFeedback(hardware, props);
            SetupFeedbacks(props);
            AddRoutingPorts();
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

        private void SetupFeedbacks(NvxDeviceProperties props)
        {
            _hdcpCapability.Add(1, Hdmi1HdcpCapabilityValueFeedback.GetFeedback(Hardware));
            _hdcpCapability.Add(2, Hdmi2HdcpCapabilityValueFeedback.GetFeedback(Hardware));
            _syncDetected.Add(1, Hdmi1SyncDetectedFeedback.GetFeedback(Hardware));
            _syncDetected.Add(2, Hdmi2SyncDetectedFeedback.GetFeedback(Hardware));

            DisabledByHdcp = HdmiOutputDisabledFeedback.GetFeedback(Hardware);
            HorizontalResolution = HorizontalResolutionFeedback.GetFeedback(Hardware);
            VideowallMode = VideowallModeFeedback.GetFeedback(Hardware);

            Feedbacks.AddRange(new Feedback[]
            {
                DisabledByHdcp,
                HorizontalResolution,
                VideowallMode,
                _syncDetected[1],
                _syncDetected[2],
                _hdcpCapability[1],
                _hdcpCapability[2],
                Hdmi1HdcpCapabilityFeedback.GetFeedback(Hardware),
                Hdmi2HdcpCapabilityFeedback.GetFeedback(Hardware)
            });

            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
            DeviceDebug.RegisterForRoutingInputPortFeedback(this);
            DeviceDebug.RegisterForRoutingOutputFeedback(this);
        }

        private void AddRoutingPorts()
        {
            HdmiInput1.AddRoutingPort(this);
            HdmiInput2.AddRoutingPort(this);
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

        public CrestronCollection<ComPort> ComPorts { get { return Hardware.ComPorts; } }
        public int NumberOfComPorts { get { return Hardware.NumberOfComPorts; } }

        public CrestronCollection<IROutputPort> IROutputPorts { get { return Hardware.IROutputPorts; } }
        public int NumberOfIROutputPorts { get { return Hardware.NumberOfIROutputPorts; } }

        public Cec StreamCec { get { return Hardware.HdmiOut.StreamCec; } }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            _router.ExecuteSwitch(inputSelector, outputSelector, signalType);
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public void UpdateDeviceId(uint id)
        {
            _device.UpdateDeviceId(id);
        }

        public new DmNvx35x Hardware { get; private set; }

        public StringFeedback CurrentStreamName
        {
            get { return _currentVideoStream.CurrentStreamName; }
        }

        public IntFeedback CurrentStreamId
        {
            get { return _currentVideoStream.CurrentStreamId; }
        }

        public StringFeedback CurrentSecondaryAudioStreamName
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamName; }
        }

        public IntFeedback CurrentSecondaryAudioStreamId
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamId; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public StringFeedback StreamUrl
        {
            get { return _currentVideoStream.StreamUrl; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _currentVideoStream.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _currentVideoStream.VideoStreamStatus; }
        }

        public StringFeedback CurrentVideoInput
        {
            get { return _device.CurrentVideoInput; }
        }

        public IntFeedback CurrentVideoInputValue
        {
            get { return _device.CurrentVideoInputValue; }
        }

        public StringFeedback CurrentAudioInput
        {
            get { return _device.CurrentAudioInput; }
        }

        public IntFeedback CurrentAudioInputValue
        {
            get { return _device.CurrentAudioInputValue; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get { return new ReadOnlyDictionary<uint, IntFeedback>(_hdcpCapability); } }
        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get { return new ReadOnlyDictionary<uint, BoolFeedback>(_syncDetected); } }

        public BoolFeedback DisabledByHdcp { get; private set; }
        public IntFeedback HorizontalResolution { get; private set; }
        public IntFeedback VideowallMode { get; private set; }

        DmNvxBaseClass INvxHardware.Hardware
        {
            get { return _device.Hardware; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var deviceBridge = new NvxDeviceBridge(this);
            deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioAddress; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _currentSecondaryAudioStream.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioStreamStatus; }
        }

        public bool IsRemote
        {
            get { return _usbStream.IsRemote; }
        }

        public StringFeedback UsbLocalId
        {
            get { return _usbStream.UsbLocalId; }
        }

        public StringFeedback UsbRemoteId
        {
            get { return _usbStream.UsbRemoteId; }
        }

        public int UsbId
        {
            get { return _usbStream.UsbId; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public override string ToString()
        {
            return Key;
        }
    }
}