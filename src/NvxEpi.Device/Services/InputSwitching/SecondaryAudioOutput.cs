﻿using System;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Services.Utilities;
using NvxEpi.Extensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.InputSwitching
{
    public class SecondaryAudioOutput : IHandleInputSwitch
    {
        public const string Key = "SecondaryAudioOutput";

        private readonly ICurrentAudioInput _device;

        public SecondaryAudioOutput(ICurrentAudioInput device)
        {
            _device = device;
        }

        public void HandleSwitch(object input, eRoutingSignalType type)
        {
            if (!_device.IsTransmitter)
                throw new NotSupportedException("receiver"); 

            var routingInput = input as DeviceInputEnum;
            if (routingInput == null)
                throw new InvalidCastException("routing input");

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
            else
                throw new NotSupportedException(input.Name);
        }

        public static void AddRoutingPort(ICurrentAudioInput parent)
        {
            parent.OutputPorts.Add(new RoutingOutputPort(
                Key, 
                eRoutingSignalType.Audio, 
                eRoutingPortConnectionType.LineAudio,
                new SecondaryAudioOutput(parent), 
                parent));
        }
    }
}