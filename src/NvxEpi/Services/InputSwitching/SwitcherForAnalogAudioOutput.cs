using System;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputSwitching
{
    public class SwitcherForAnalogAudioOutput : IHandleInputSwitch
    {
        public const string Key = "AnalogAudioOutput";

        private readonly ICurrentAudioInput _device;

        public SwitcherForAnalogAudioOutput(ICurrentAudioInput device)
        {
            _device = device;
        }

        public void HandleSwitch(object input, eRoutingSignalType type)
        {
            var routingInput = input as DeviceInputEnum;
            if (routingInput == null)
                throw new InvalidCastException("routing input");

            Debug.Console(1, _device, "Switching input on AnalogAudioOutput: '{0}' : '{1}'", routingInput.Name, type.ToString());

            if (routingInput == DeviceInputEnum.NoSwitch)
                return;

            if (type.Has(eRoutingSignalType.Audio))
                SwitchAudio(routingInput);

            if (type.Has(eRoutingSignalType.Video))
                throw new NotSupportedException("video"); 
        }

        private void SwitchAudio(Enumeration<DeviceInputEnum> input)
        {
            if (input == DeviceInputEnum.PrimaryAudio)
                _device.SetAudioToPrimaryStreamAudio();
            else if (input == DeviceInputEnum.Stream)
                _device.SetAudioToPrimaryStreamAudio();
            else if (input == DeviceInputEnum.SecondaryAudio)
                _device.SetAudioToSecondaryStreamAudio();
            else if (input == DeviceInputEnum.Hdmi1)
                _device.SetAudioToHdmiInput1();
            else if (input == DeviceInputEnum.Hdmi2)
                _device.SetAudioToHdmiInput2();
            else if (input == DeviceInputEnum.DmNaxAudio)
                _device.SetAudioToSecondaryStreamAudio();
            else
                throw new NotSupportedException(input.Name);
        }

        public override string ToString()
        {
            return _device.Key + "-" + Key;
        }

        public static void AddRoutingPort(ICurrentAudioInput parent)
        {
            parent.OutputPorts.Add(new RoutingOutputPort(
                Key, 
                eRoutingSignalType.Audio, 
                eRoutingPortConnectionType.LineAudio,
                new SwitcherForAnalogAudioOutput(parent), 
                parent));
        }
    }
}