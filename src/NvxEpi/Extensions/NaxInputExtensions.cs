using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class NaxInputExtensions
    {
        public static void SetNaxInput(this ICurrentNaxInput device, ushort input)
        {
            var inputToSwitch = (DmNvxControl.eAudioSource) input;

            switch (inputToSwitch)
            {
                case DmNvxControl.eAudioSource.Automatic:
                    device.SetNaxAudioToInputAutomatic();
                    break;
                case DmNvxControl.eAudioSource.Input1:
                    device.SetNaxAudioToHdmiInput1();
                    break;
                case DmNvxControl.eAudioSource.Input2:
                    device.SetNaxAudioToHdmiInput2();
                    break;
                case DmNvxControl.eAudioSource.AnalogAudio:
                    device.SetNaxAudioToInputAnalog();
                    break;
                case DmNvxControl.eAudioSource.PrimaryStreamAudio:
                    device.SetNaxAudioToPrimaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.DmNaxAudio:
                    device.SetNaxAudioToSecondaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.DanteAes67Audio:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(input.ToString());
            }
        }

        public static void SetNaxAudioToHdmiInput1(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Hdmi1'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public static void SetNaxAudioToHdmiInput2(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Hdmi2'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public static void SetNaxAudioToInputAnalog(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Analog'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }

        public static void SetNaxAudioToPrimaryStreamAudio(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'PrimaryStream'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        public static void SetNaxAudioToSecondaryStreamAudio(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'SecondaryStream'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
        }

        public static void SetNaxAudioToInputAutomatic(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Automatic'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Automatic;
        }
    }
}