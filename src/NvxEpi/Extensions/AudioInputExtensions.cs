using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class AudioInputExtensions
    {
        public static void SetAudioInput(this ICurrentAudioInput device, ushort input)
        {
            var inputToSwitch = (DmNvxControl.eAudioSource) input;

            switch (inputToSwitch)
            {
                case DmNvxControl.eAudioSource.Automatic:
                    device.SetAudioToInputAutomatic();
                    break;
                case DmNvxControl.eAudioSource.Input1:
                    device.SetAudioToHdmiInput1();
                    break;
                case DmNvxControl.eAudioSource.Input2:
                    device.SetAudioToHdmiInput2();
                    break;
                case DmNvxControl.eAudioSource.AnalogAudio:
                    device.SetAudioToInputAnalog();
                    break;
                case DmNvxControl.eAudioSource.PrimaryStreamAudio:
                    device.SetAudioToPrimaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.SecondaryStreamAudio:
                    device.SetAudioToSecondaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.DanteAes67Audio:
                    throw new NotImplementedException();
                case DmNvxControl.eAudioSource.DmNaxAudio:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(input.ToString());
            }
        }

        public static void SetAudioToHdmiInput1(this ICurrentAudioInput device)
        {
            Debug.Console(1, device, "Switching Audio Input to : 'Hdmi1'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public static void SetAudioToHdmiInput2(this ICurrentAudioInput device)
        {
            if (!(device.Hardware is DmNvx35x))
                return;

            Debug.Console(1, device, "Switching Audio Input to : 'Hdmi2'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public static void SetAudioToInputAnalog(this ICurrentAudioInput device)
        {
            if (!device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching Audio Input to : 'Analog'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }

        public static void SetAudioToPrimaryStreamAudio(this ICurrentAudioInput device)
        {
            if (!(device.Hardware is DmNvx35x))
                return;

            if (!device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching Audio Input to : 'PrimaryStream'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        public static void SetAudioToSecondaryStreamAudio(this ICurrentAudioInput device)
        {
            if (!(device.Hardware is DmNvx35x))
                return;

            if (!device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching Audio Input to : 'SecondaryStream'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
        }

        public static void SetAudioToInputAutomatic(this ICurrentAudioInput device)
        {
            Debug.Console(1, device, "Switching Audio Input to : 'Automatic'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
        }
    }
}