using System;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputSwitching
{
    public class SwitcherForHdmiOutput : IHandleInputSwitch
    {
        public const string Key = "HdmiOutput";
        private readonly ICurrentVideoInput _device;

        public SwitcherForHdmiOutput(ICurrentVideoInput device)
        {
            _device = device;
        }

        public void HandleSwitch(object input, eRoutingSignalType type)
        {
            if (_device.IsTransmitter)
                throw new NotSupportedException("transmitter");

            var routingInput = input as DeviceInputEnum;
            if (routingInput == null)
                throw new InvalidCastException("routing input");

            Debug.Console(1, _device, "Switching input on HdmiOutput: '{0}' : '{1}", routingInput.Name, type.ToString());

            if (type.Is(eRoutingSignalType.AudioVideo))
            {
                SwitchVideo(routingInput);

                var deviceWithAudioSwitching = _device as ICurrentAudioInput;
                if (deviceWithAudioSwitching != null)
                    deviceWithAudioSwitching.SetAudioToInputAutomatic();

                return;
            }

            if (type.Has(eRoutingSignalType.Video))
                SwitchVideo(routingInput);

            if (type.Has(eRoutingSignalType.Audio))
                SwitchAudio(routingInput);
        }

        private void SwitchVideo(Enumeration<DeviceInputEnum> input)
        {
            if (input == DeviceInputEnum.Stream)
                _device.SetVideoToStream();
            else if (input == DeviceInputEnum.Hdmi1)
                _device.SetVideoToHdmiInput1();
            else if (input == DeviceInputEnum.Hdmi2)
                _device.SetVideoToHdmiInput2();
            else
                throw new NotSupportedException(input.Name);
        }

        private void SwitchAudio(Enumeration<DeviceInputEnum> input)
        {
            var deviceWithAudioSwitching = _device as ICurrentAudioInput;
            if (deviceWithAudioSwitching == null) return;

            if (input == DeviceInputEnum.Stream)
                deviceWithAudioSwitching.SetAudioToPrimaryStreamAudio();
            else if (input == DeviceInputEnum.PrimaryAudio)
                deviceWithAudioSwitching.SetAudioToPrimaryStreamAudio();
            else if (input == DeviceInputEnum.SecondaryAudio)
                deviceWithAudioSwitching.SetAudioToSecondaryStreamAudio();
            else if (input == DeviceInputEnum.Hdmi1)
                deviceWithAudioSwitching.SetAudioToHdmiInput1();
            else if (input == DeviceInputEnum.Hdmi2)
                deviceWithAudioSwitching.SetAudioToHdmiInput2();
            else if (input == DeviceInputEnum.AnalogAudio)
                deviceWithAudioSwitching.SetAudioToInputAnalog();
            else
                throw new NotSupportedException(input.Name);
        }

        public override string ToString()
        {
            return _device.Key + "-" + Key;
        }

        public static void AddRoutingPort(ICurrentVideoInput parent)
        {
            parent.OutputPorts.Add(new RoutingOutputPort(
                Key,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                new SwitcherForHdmiOutput(parent),
                parent) { Port = parent });
        }
    }
}