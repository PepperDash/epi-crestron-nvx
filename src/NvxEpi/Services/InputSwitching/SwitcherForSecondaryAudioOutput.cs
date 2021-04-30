using System;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.InputSwitching
{
    public class SwitcherForSecondaryAudioOutput : IHandleInputSwitch
    {
        public const string Key = "SecondaryAudioOutput";

        private readonly ICurrentNaxInput _device;

        public SwitcherForSecondaryAudioOutput(ICurrentNaxInput device)
        {
            _device = device;
        }

        public void HandleSwitch(object input, eRoutingSignalType type)
        {
            var routingInput = input as DeviceInputEnum;
            if (routingInput == null)
                throw new InvalidCastException("routing input");

            if (routingInput == DeviceInputEnum.NoSwitch)
                return;

            Debug.Console(1, _device, "Switching input on SecondaryAudioOutput: '{0}' : '{1}", routingInput.Name, type.ToString());

            if (type.Has(eRoutingSignalType.Audio))
                SwitchAudio(routingInput);

            if (type.Has(eRoutingSignalType.Video))
                throw new NotSupportedException("video"); 
        }

        private void SwitchAudio(Enumeration<DeviceInputEnum> input)
        {
            if (input == DeviceInputEnum.Hdmi1)
                _device.SetNaxAudioToHdmiInput1();
            else if (input == DeviceInputEnum.Hdmi2)
                _device.SetNaxAudioToHdmiInput2();
            else if (input == DeviceInputEnum.AnalogAudio)
                _device.SetNaxAudioToInputAnalog();
            else if (input == DeviceInputEnum.Automatic)
                _device.SetNaxAudioToInputAutomatic();
            else
                throw new NotSupportedException(input.Name);
        }

        public override string ToString()
        {
            return _device.Key + "-" + Key;
        }

        public static void AddRoutingPort(ICurrentNaxInput parent)
        {
            parent.OutputPorts.Add(new RoutingOutputPort(
                Key, 
                eRoutingSignalType.Audio, 
                eRoutingPortConnectionType.LineAudio,
                new SwitcherForSecondaryAudioOutput(parent), 
                parent));
        }
    }
}