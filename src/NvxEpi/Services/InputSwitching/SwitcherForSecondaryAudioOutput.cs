﻿using System;
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

        private readonly ICurrentAudioInput _device;

        public SwitcherForSecondaryAudioOutput(ICurrentAudioInput device)
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
                _device.SetAudioToHdmiInput1();
            else if (input == DeviceInputEnum.Hdmi2)
                _device.SetAudioToHdmiInput2();
            else if (input == DeviceInputEnum.AnalogAudio)
                _device.SetAudioToInputAnalog();
            else if (input == DeviceInputEnum.Automatic)
                _device.SetAudioToInputAutomatic();
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
                new SwitcherForSecondaryAudioOutput(parent), 
                parent));
        }
    }
}