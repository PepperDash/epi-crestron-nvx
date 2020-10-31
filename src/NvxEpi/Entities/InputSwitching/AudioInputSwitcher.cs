using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.InputSwitching
{
    public class AudioInputSwitcher : ICurrentAudioInput
    {
        private readonly INvxHardware _device;

        public AudioInputSwitcher(INvxHardware device)
        {
            _device = device;
            Initialize();
        }

        public string Key { get { return _device.Key; } }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get { return _device.InputPorts; } }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get { return _device.OutputPorts; } }

        public IntFeedback DeviceMode { get { return _device.DeviceMode; } }

        public bool IsTransmitter { get { return _device.IsTransmitter; } }

        public string Name { get { return _device.Name; } }

        public int DeviceId { get { return _device.DeviceId; } }

        public DmNvxBaseClass Hardware { get { return _device.Hardware; } }

        public StringFeedback MulticastAddress { get { return _device.MulticastAddress; } }

        public StringFeedback CurrentAudioInput { get; private set; }

        public IntFeedback CurrentAudioInputValue { get; private set; }

        public FeedbackCollection<Feedback> Feedbacks { get { return _device.Feedbacks; } }

        public BoolFeedback IsOnline { get { return _device.IsOnline; } }

        public StringFeedback VideoName { get { return _device.VideoName; } }

        public StringFeedback AudioName { get { return _device.AudioName; } }

        public void SetAudioInput(ushort input)
        {
            var inputToSwitch = (DmNvxControl.eAudioSource) input;

            switch (inputToSwitch)
            {
                case DmNvxControl.eAudioSource.Automatic:
                    SetAudioToInputAutomatic();
                    break;
                case DmNvxControl.eAudioSource.Input1:
                    SetAudioToHdmiInput1();
                    break;
                case DmNvxControl.eAudioSource.Input2:
                    SetAudioToHdmiInput2();
                    break;
                case DmNvxControl.eAudioSource.AnalogAudio:
                    SetAudioToInputAnalog();
                    break;
                case DmNvxControl.eAudioSource.PrimaryStreamAudio:
                    SetAudioToPrimaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.SecondaryStreamAudio:
                    SetAudioToSecondaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.DanteAes67Audio:
                    throw new NotImplementedException();
                case DmNvxControl.eAudioSource.DmNaxAudio:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(input.ToString());
            }
        }

        public void SetAudioToHdmiInput1()
        {
            Debug.Console(1, this, "Switching Audio Input to : 'Hdmi1'");
            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public void SetAudioToHdmiInput2()
        {
            if (!( Hardware is DmNvx35x ))
                return;

            Debug.Console(1, this, "Switching Audio Input to : 'Hdmi2'");
            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public void SetAudioToInputAnalog()
        {
            if (!IsTransmitter)
                return;

            Debug.Console(1, this, "Switching Audio Input to : 'Analog'");
            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }

        public void SetAudioToPrimaryStreamAudio()
        {
            if (!( Hardware is DmNvx35x ))
                return;

            if (!IsTransmitter)
                return;

            Debug.Console(1, this, "Switching Audio Input to : 'PrimaryStream'");
            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        public void SetAudioToSecondaryStreamAudio()
        {
            if (!( Hardware is DmNvx35x ))
                return;

            if (!IsTransmitter)
                return;

            Debug.Console(1, this, "Switching Audio Input to : 'SecondaryStream'");
            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
        }

        public void SetAudioToInputAutomatic()
        {
            Debug.Console(1, this, "Switching Audio Input to : 'Automatic'");
            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
        }

        private void Initialize()
        {
            CurrentAudioInput = AudioInputFeedback.GetFeedback(Hardware);
            CurrentAudioInputValue = AudioInputValueFeedback.GetFeedback(Hardware);

            _device.Feedbacks.Add(CurrentAudioInput);
            _device.Feedbacks.Add(CurrentAudioInputValue);
        }
    }
}