using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Hardware;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.Services.Feedback;
using NvxEpi.Device.Services.InputPorts;
using NvxEpi.Device.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Entities.Aggregates
{
    public class Nvx35x : NvxBaseDevice, IComPorts, IIROutputPorts, ICurrentStream, ICurrentVideoInput, ICurrentAudioInput,
        ICurrentSecondaryAudioStream, IHdmiInput, IVideowallMode, IRouting
    {
        private readonly Nvx35xHardware _device;
        private readonly ICurrentStream _currentVideoStream;
        private readonly ISecondaryAudioStream _secondaryAudioStream;
        private readonly ICurrentSecondaryAudioStream _currentSecondaryAudioStream;

        private readonly Dictionary<uint, IntFeedback> _hdcpCapability = 
            new Dictionary<uint, IntFeedback>();

        private readonly Dictionary<uint, BoolFeedback> _syncDetected = 
            new Dictionary<uint, BoolFeedback>();

        public Nvx35x(DeviceConfig config, DmNvx35x hardware)
            : base(config, hardware)
        {
            Hardware = hardware;
            _device = new Nvx35xHardware(config, hardware, Feedbacks, IsOnline);

            var stream = new VideoStream(_device);
            _currentVideoStream = new CurrentVideoStream(stream);
            _secondaryAudioStream = new SecondaryAudioStream(_device);
            _currentSecondaryAudioStream = new CurrentSecondaryAudioStream(_secondaryAudioStream);

            AddRoutingPorts();
            SetupFeedbacks();
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
                Hdmi2HdcpCapabilityFeedback.GetFeedback(Hardware),
                _currentVideoStream.CurrentStreamName,
                _currentVideoStream.CurrentStreamId,
                _currentSecondaryAudioStream.CurrentSecondaryAudioStreamId,
                _currentSecondaryAudioStream.CurrentSecondaryAudioStreamName
            });
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
            try
            {
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("input selector");

                switcher.HandleSwitch(inputSelector, signalType);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error executing route! : {0}", ex.Message);
            }
        }

        public override IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public override bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public override int DeviceId
        {
            get { return _device.DeviceId; }
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

        public StringFeedback SecondaryAudioAddress
        {
            get { return _secondaryAudioStream.SecondaryAudioAddress; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _secondaryAudioStream.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _secondaryAudioStream.SecondaryAudioStreamStatus; }
        }

        public StringFeedback CurrentSecondaryAudioStreamName
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamName; }
        }

        public IntFeedback CurrentSecondaryAudioStreamId
        {
            get { return _currentSecondaryAudioStream.CurrentSecondaryAudioStreamId; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get { return new ReadOnlyDictionary<uint, IntFeedback>(_hdcpCapability); } }
        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get { return new ReadOnlyDictionary<uint, BoolFeedback>(_syncDetected); } }

        public BoolFeedback DisabledByHdcp { get; private set; }
        public IntFeedback HorizontalResolution { get; private set; }
        public IntFeedback VideowallMode { get; private set; }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _device.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _device.VideoStreamStatus; }
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
    }
}