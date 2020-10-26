using System;
using System.Globalization;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.NaxAudio;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class NaxAudioInputExtensions
    {
        public static void SetAudioInput(this INaxAudioStream device, ushort input)
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
                    throw new ArgumentOutOfRangeException(input.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static void SetAudioToHdmiInput1(this INaxAudioStream device)
        {
            Debug.Console(1, device, "Switching NAX Audio Input to : 'Hdmi1'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public static void SetAudioToHdmiInput2(this INaxAudioStream device)
        {
            if (!(device.Hardware is DmNvx35x))
                return;

            Debug.Console(1, device, "Switching NAX Audio Input to : 'Hdmi2'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public static void SetAudioToInputAnalog(this INaxAudioStream device)
        {
            if (!device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching NAX Audio Input to : 'Analog'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }

        public static void SetAudioToPrimaryStreamAudio(this INaxAudioStream device)
        {
            if (!(device.Hardware is DmNvx35x))
                return;

            Debug.Console(1, device, "Switching NAX Audio Input to : 'PrimaryStream'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        public static void SetAudioToSecondaryStreamAudio(this INaxAudioStream device)
        {
            if (!(device.Hardware is DmNvx35x))
                return;

            Debug.Console(1, device, "Switching NAX Audio Input to : 'SecondaryStream'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
        }

        public static void SetAudioToInputAutomatic(this INaxAudioStream device)
        {
            Debug.Console(1, device, "Switching NAX Audio Input to : 'Automatic'");
            device.Hardware.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Automatic;
        }
    }
}