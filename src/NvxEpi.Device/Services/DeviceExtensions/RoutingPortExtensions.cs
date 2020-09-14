using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Ports;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class RoutingPortExtensions
    {
        public static INvxDevice AddStreams(this INvxDevice device)
        {
            return device.IsTransmitter.BoolValue ? device.AddStreamOutputs() : device.AddStreamInputs();
        }

        private static INvxDevice AddStreamOutputs(this INvxDevice device)
        {
            var streamKey = eSfpVideoSourceTypes.Stream.ToString();
            device.OutputPorts.Add(new RoutingOutputPort(
                streamKey,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                null,
                device));

            return device;
        }

        private static INvxDevice AddStreamInputs(this INvxDevice device)
        {
            var videoStreamKey = eSfpVideoSourceTypes.Stream.ToString();
            device.InputPorts.Add(new RoutingInputPort(
                videoStreamKey,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                new Action<eRoutingSignalType>(type =>
                {
                    Debug.Console(1, device, "Making an awesome route of type {0} : 'VideoStream'", type);

                    if (type.Has(eRoutingSignalType.Video))
                        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;

                    if (type.Has(eRoutingSignalType.Audio))
                        device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                }),
                device));

            return device;
        }

        public static INvxDevice AddSecondaryAudio(this INvxDevice device)
        {
            if (device.IsTransmitter.BoolValue)
                return device;

            if (device.Hardware is DmNvxD3x || device.Hardware is DmNvxE3x)
                return device;

            var audioStreamKey = DmNvxControl.eAudioSource.SecondaryStreamAudio.ToString();
            device.InputPorts.Add(new RoutingInputPort(
                audioStreamKey,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.Streaming,
                new Action<eRoutingSignalType>(type =>
                {
                    Debug.Console(1, device, "Making an awesome route of type {0} : 'SecondaryAudioStream'", type);
                    if (type.Has(eRoutingSignalType.Audio))
                        device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
                }),
                device));

            return device;
        }

        public static INvxDevice AddHdmiInput1(this INvxDevice device)
        {
            var key = eSfpVideoSourceTypes.Hdmi1.ToString();
            device.InputPorts.Add(new RoutingInputPort(
                key,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                new Action<eRoutingSignalType>(type =>
                {
                    Debug.Console(1, device, "Making an awesome route of type {0} : 'Hdmi1'", type);

                    if (type.Has(eRoutingSignalType.Video))
                        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;

                    if (type.Has(eRoutingSignalType.Audio))
                        device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
                }),
                device));

            return device;
        }

        public static INvxDevice AddHdmiInput2(this INvxDevice device)
        {
            var key = eSfpVideoSourceTypes.Hdmi1.ToString();
            device.InputPorts.Add(new RoutingInputPort(
                key,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                new Action<eRoutingSignalType>(type =>
                {
                    Debug.Console(1, device, "Making an awesome route of type {0} : 'Hdmi2'", type);

                    if (type.Has(eRoutingSignalType.Video))
                        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;

                    if (type.Has(eRoutingSignalType.Audio))
                        device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
                }),
                device));

            return device;
        }

        public static INvxDevice AddHdmiOutput(this INvxDevice device)
        {
            if (device.Hardware.HdmiOut == null)
                return device;

            device.OutputPorts.Add(new RoutingOutputPort(
                "HdmiOut",
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                new HdmiOutputRoutingCommand(device),
                device));

            return device;
        }

        public static INvxDevice AddNaxAudio(this INvxDevice device)
        {
            var audioTxStreamKey = DmNvxControl.eAudioSource.DmNaxAudio + "Tx";
            device.OutputPorts.Add(new RoutingOutputPort(
                audioTxStreamKey,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.Streaming,
                null,
                device));

            var audioRxStreamKey = DmNvxControl.eAudioSource.DmNaxAudio + "Rx";
            device.InputPorts.Add(new RoutingInputPort(
                audioRxStreamKey,
                eRoutingSignalType.Audio,
                eRoutingPortConnectionType.Streaming,
                new Action<eRoutingSignalType>(type =>
                {
                    Debug.Console(1, device, "Making an awesome route of type {0} : 'NaxAudioStream'", type);
                    if (type.Has(eRoutingSignalType.Audio))
                        device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                }),
                device));

            return device;
        }
    }
}