﻿using System;
using NvxEpi.Abstractions.Extensions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.InputSwitching
{
    public class StreamOutput : IHandleInputSwitch
    {
        public const string Key = "StreamOutput";

        private readonly ICurrentVideoInput _device;

        public StreamOutput(ICurrentVideoInput device)
        {
            _device = device;
        }

        public void HandleSwitch(object input, eRoutingSignalType type)
        {
            if (!_device.IsTransmitter)
                throw new NotSupportedException("receiver");

            if (_device.Hardware.HdmiOut == null)
                throw new NotSupportedException("hdmi out");

            var routingInput = input as DeviceInputEnum;
            if (routingInput == null)
                throw new InvalidCastException("routing input");

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
            if (input == DeviceInputEnum.Hdmi1)
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

            if (input == DeviceInputEnum.PrimaryAudio)
                deviceWithAudioSwitching.SetAudioToPrimaryStreamAudio();
            else if (input == DeviceInputEnum.SecondaryAudio)
                deviceWithAudioSwitching.SetAudioToSecondaryStreamAudio();
            else if (input == DeviceInputEnum.Hdmi1)
                deviceWithAudioSwitching.SetAudioToHdmiInput1();
            else if (input == DeviceInputEnum.Hdmi2)
                deviceWithAudioSwitching.SetAudioToHdmiInput2();
            else
                throw new NotSupportedException(input.Name);
        }

        public static void AddRoutingPort(ICurrentVideoInput parent)
        {
            parent.OutputPorts.Add(new RoutingOutputPort(
                Key, 
                eRoutingSignalType.AudioVideo, 
                eRoutingPortConnectionType.Streaming,
                new StreamOutput(parent), 
                parent));
        }
    }
}