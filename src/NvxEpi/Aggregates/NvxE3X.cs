using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
using NvxEpi.Entities.Hardware;
using NvxEpi.Entities.Streams;
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
    public class NvxE3X : CrestronGenericBridgeableBaseDevice, INvxE3XHardware, IComPorts, IIROutputPorts, ICurrentStream,
        ICurrentVideoInput, ICurrentAudioInput, IHdmiInput, IRouting
    {
        private readonly NvxE3XHardware _device;
        private readonly ICurrentStream _currentVideoStream;
        private readonly IUsbStream _usbStream;

        private readonly Dictionary<uint, IntFeedback> _hdcpCapability = 
            new Dictionary<uint, IntFeedback>();

        private readonly Dictionary<uint, BoolFeedback> _syncDetected = 
            new Dictionary<uint, BoolFeedback>();

        public NvxE3X(DeviceConfig config, DmNvxE3x hardware)
            : base(config.Key, config.Name, hardware)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            Hardware = hardware;

            _device = new NvxE3XHardware(config, hardware, Feedbacks, IsOnline);
            _currentVideoStream = new CurrentVideoStream(new VideoStream(_device));

            RegisterForOnlineFeedback(hardware, props);
            SetupFeedbacks();
            AddRoutingPorts();
        }

        private void RegisterForOnlineFeedback(GenericBase hardware, NvxDeviceProperties props)
        {
            hardware.OnlineStatusChange += (device, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                Hardware.Control.Name.StringValue = Name.Replace(' ', '-');
                Hardware.SetDefaults(props);
            };
        }

        private void SetupFeedbacks()
        {
            _hdcpCapability.Add(1, Hdmi1HdcpCapabilityValueFeedback.GetFeedback(Hardware));
            _syncDetected.Add(1, Hdmi1SyncDetectedFeedback.GetFeedback(Hardware));

            Feedbacks.AddRange(new Feedback[]
            {
                _syncDetected[1],
                _hdcpCapability[1],
                Hdmi1HdcpCapabilityFeedback.GetFeedback(Hardware)
            });

            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
            DeviceDebug.RegisterForRoutingInputPortFeedback(this);
            DeviceDebug.RegisterForRoutingOutputFeedback(this);
        }

        private void AddRoutingPorts()
        {
            HdmiInput1.AddRoutingPort(this);
            StreamOutput.AddRoutingPort(this);
            AnalogAudioInput.AddRoutingPort(this);
        }

        public CrestronCollection<ComPort> ComPorts { get { return Hardware.ComPorts; } }
        public int NumberOfComPorts { get { return Hardware.NumberOfComPorts; } }

        public CrestronCollection<IROutputPort> IROutputPorts { get { return Hardware.IROutputPorts; } }
        public int NumberOfIROutputPorts { get { return Hardware.NumberOfIROutputPorts; } }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("input selector");

                Debug.Console(1, this, "Executing switch : '{0}' | '{1}' | '{2}'", inputSelector.ToString(), outputSelector.ToString(), signalType.ToString());
                switcher.HandleSwitch(inputSelector, signalType);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error executing switch! : {0}", ex.Message);
            }
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

        public StringFeedback CurrentStreamName
        {
            get { return _currentVideoStream.CurrentStreamName; }
        }

        public IntFeedback CurrentStreamId
        {
            get { return _currentVideoStream.CurrentStreamId; }
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

        public new DmNvxE3x Hardware { get; private set; }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _device.SecondaryAudioAddress; }
        }
    }
}