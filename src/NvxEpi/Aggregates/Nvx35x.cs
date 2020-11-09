using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
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
    public class Nvx35X : NvxBaseDevice, IComPorts, IIROutputPorts, ICurrentStream, IUsbStream,
        ICurrentSecondaryAudioStream, IHdmiInput, IVideowallMode, IRouting, ICec
    {
        private readonly ICurrentSecondaryAudioStream _currentSecondaryAudioStream;
        private readonly ICurrentStream _currentVideoStream;

        private readonly Dictionary<uint, IntFeedback> _hdcpCapability =
            new Dictionary<uint, IntFeedback>();

        private readonly Dictionary<uint, BoolFeedback> _syncDetected =
            new Dictionary<uint, BoolFeedback>();

        private readonly IUsbStream _usbStream;

        public Nvx35X(DeviceConfig config, DmNvx35x hardware)
            : base(config, hardware)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            Hardware = hardware;

            _currentVideoStream = new CurrentVideoStream(new VideoStream(this));
            _currentSecondaryAudioStream = new CurrentSecondaryAudioStream(new SecondaryAudioStream(this));

            RegisterForOnlineFeedback(hardware, props);
            SetupFeedbacks();
            AddRoutingPorts();
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
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

        public BoolFeedback DisabledByHdcp { get; private set; }

        public new DmNvx35x Hardware { get; private set; }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
        {
            get { return new ReadOnlyDictionary<uint, IntFeedback>(_hdcpCapability); }
        }

        public IntFeedback HorizontalResolution { get; private set; }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return Hardware.IROutputPorts; }
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

        public int NumberOfComPorts
        {
            get { return Hardware.NumberOfComPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return Hardware.NumberOfIROutputPorts; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _currentSecondaryAudioStream.SecondaryAudioStreamStatus; }
        }

        public Cec StreamCec
        {
            get { return Hardware.HdmiOut.StreamCec; }
        }

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
        {
            get { return new ReadOnlyDictionary<uint, BoolFeedback>(_syncDetected); }
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

        public StringFeedback VideoStreamStatus
        {
            get { return _currentVideoStream.VideoStreamStatus; }
        }

        public IntFeedback VideowallMode { get; private set; }

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
    }
}