using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Device;
using NvxEpi.Device.Services.DeviceExtensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Aggregates
{
    public class Nvx35x : NvxBaseDevice, INvx35x, IHasCurrentVideoInput, IHasCurrentAudioInput, IHasHdmiInputs, IHasHdmiOutput, 
        IHasVideoStreamRouting, IHasSecondaryAudioStreamRouting, IRouting, IComPorts, IIROutputPorts, ICec
    {
        private readonly IHasCurrentVideoInput _currentVideoInput;
        private readonly IHasCurrentAudioInput _currentAudioInput;
        private readonly IHasHdmiInputs _hdmiInputs;
        private readonly IHasHdmiOutput _hasHdmiOutput;
        private readonly IHasVideoStreamRouting _videoStreamRouting;
        private readonly IHasSecondaryAudioStreamRouting _secondaryAudioStreamRouting;

        public Nvx35x(INvxDevice device)
            : base(device)
        {
            var hardware = device.Hardware as DmNvx35x;
            if (hardware == null)
                throw new ArgumentNullException("hardware as Nvx35x");

            Hardware = hardware;

            _currentVideoInput = new CurrentVideoInput(_device);
            _currentAudioInput = new CurrentCurrentAudioInput(_device);
            _videoStreamRouting = new PrimaryVideoStream(_device);
            _secondaryAudioStreamRouting = new CurrentSecondaryAudio(_device);
            _hdmiInputs = new DeviceHdmiInputs(_device);
            _hasHdmiOutput = new DeviceHdmiOutput(_device);

            _device.AddHdmiInput1();
            _device.AddHdmiInput2();
            _device.AddSecondaryAudio();
        }

        public CrestronCollection<ComPort> ComPorts { get { return _device.Hardware.ComPorts; } }
        public int NumberOfComPorts { get { return _device.Hardware.NumberOfComPorts; } }

        public CrestronCollection<IROutputPort> IROutputPorts { get { return _device.Hardware.IROutputPorts; } }
        public int NumberOfIROutputPorts { get { return _device.Hardware.NumberOfIROutputPorts; } }

        public Cec StreamCec { get { return _device.Hardware.HdmiOut.StreamCec; } }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var action = inputSelector as Action<eRoutingSignalType>;
            if (action == null)
                throw new NullReferenceException("action");

            action(signalType);
        }

        public new DmNvx35x Hardware { get; private set; }

        public IntFeedback CurrentSecondaryAudioRouteValue
        {
            get { return _secondaryAudioStreamRouting.CurrentSecondaryAudioRouteValue; }
        }

        public StringFeedback CurrentSecondaryAudioRouteName
        {
            get { return _secondaryAudioStreamRouting.CurrentSecondaryAudioRouteName; }
        }

        public StringFeedback VideoInputName
        {
            get { return _currentVideoInput.VideoInputName; }
        }

        public IntFeedback VideoInputValue
        {
            get { return _currentVideoInput.VideoInputValue; }
        }

        public StringFeedback AudioInputName
        {
            get { return _currentAudioInput.AudioInputName; }
        }

        public IntFeedback AudioInputValue
        {
            get { return _currentAudioInput.AudioInputValue; }
        }

        public ReadOnlyDictionary<uint, DeviceHdmiInput> HdmiInputs
        {
            get { return _hdmiInputs.HdmiInputs; }
        }

        public IntFeedback HorizontalResolution
        {
            get { return _hasHdmiOutput.HorizontalResolution; }
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _hasHdmiOutput.DisabledByHdcp; }
        }

        public IntFeedback CurrentVideoRouteValue
        {
            get { return _videoStreamRouting.CurrentVideoRouteValue; }
        }

        public StringFeedback CurrentVideoRouteName
        {
            get { return _videoStreamRouting.CurrentVideoRouteName; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _secondaryAudioStreamRouting.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _secondaryAudioStreamRouting.SecondaryAudioStreamStatus; }
        }

        public StringFeedback SecondaryAudioMulticastAddress
        {
            get { return _secondaryAudioStreamRouting.SecondaryAudioMulticastAddress; }
        }
    }
}