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
                case DmNvxControl.eAudioSource.DmNaxAudio:
                    device.SetAudioToSecondaryStreamAudio();
                    break;
                case DmNvxControl.eAudioSource.DanteAes67Audio:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(input.ToString());
            }
        }

        public static void SetAudioToHdmiInput1(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Hdmi1'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public static void SetAudioToHdmiInput2(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Hdmi2'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public static void SetAudioToInputAnalog(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Analog'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }

        public static void SetAudioToPrimaryStreamAudio(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'PrimaryStream'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        public static void SetAudioToSecondaryStreamAudio(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'SecondaryStream'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
        }

        public static void SetAudioToInputAutomatic(this ICurrentNaxInput device)
        {
            Debug.Console(1, device, "Switching Nax Input to : 'Automatic'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Automatic;
        }
    }
}