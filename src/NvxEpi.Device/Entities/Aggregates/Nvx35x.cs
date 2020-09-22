using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.Services.Feedback;
using NvxEpi.Device.Services.InputSwitching;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Entities.Aggregates
{
    public class Nvx35x : NvxBaseDevice, IComPorts, IIROutputPorts, ICurrentStream, ICurrentSecondaryAudioStream, IHdmiInput, IVideowallMode
    {
        private readonly ICurrentStream _currentVideoStream;
        private readonly ICurrentSecondaryAudioStream _secondaryAudioStream;

        private readonly Dictionary<uint, IntFeedback> _hdcpCapability = 
            new Dictionary<uint, IntFeedback>();

        private readonly Dictionary<uint, BoolFeedback> _syncDetected = 
            new Dictionary<uint, BoolFeedback>();

        public Nvx35x(DeviceConfig config, DmNvx35x hardware)
            : base(config, hardware)
        {
            Hardware = hardware;
            _currentVideoStream = new CurrentVideoStream(this);
            _secondaryAudioStream = new CurrentSecondaryAudioStream(this);
        }

        public override bool CustomActivate()
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

            return base.CustomActivate();
        }

        public CrestronCollection<ComPort> ComPorts { get { return Hardware.ComPorts; } }
        public int NumberOfComPorts { get { return Hardware.NumberOfComPorts; } }

        public CrestronCollection<IROutputPort> IROutputPorts { get { return Hardware.IROutputPorts; } }
        public int NumberOfIROutputPorts { get { return Hardware.NumberOfIROutputPorts; } }

        public Cec StreamCec { get { return Hardware.HdmiOut.StreamCec; } }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var inputSwitcher = inputSelector as IHandleInputSwitch;
            if (inputSwitcher == null)
                throw new NullReferenceException("input selector");

            inputSwitcher.HandleSwitch(inputSelector, signalType);
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
            get { return _secondaryAudioStream.CurrentSecondaryAudioStreamName; }
        }

        public IntFeedback CurrentSecondaryAudioStreamId
        {
            get { return _secondaryAudioStream.CurrentSecondaryAudioStreamId; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get { return new ReadOnlyDictionary<uint, IntFeedback>(_hdcpCapability); } }
        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get { return new ReadOnlyDictionary<uint, BoolFeedback>(_syncDetected); } }

        public BoolFeedback DisabledByHdcp { get; private set; }
        public IntFeedback HorizontalResolution { get; private set; }
        public IntFeedback VideowallMode { get; private set; }
    }
}