using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class AudioInputExtensions
    {
        //TODO: needs documentation
        public static void SetAudioInput(this ICurrentAudioInput device, ushort input)
        {
            switch (input)
            {
                case 1:
                    device.SetAudioToHdmiInput1();
                    break;
                case 2:
                    device.SetAudioToHdmiInput2();
                    break;
                case 3:
                    device.SetAudioToInputAnalog();
                    break;
                case 4:
                    device.SetAudioToPrimaryStreamAudio();
                    break;
                case 5:
                    device.SetAudioToSecondaryStreamAudio();
                    break;
                case 99:
                    device.SetAudioToInputAutomatic();
                    break;
            }
        }

        public static void SetAudioToHdmiInput1(this ICurrentAudioInput device)
        {
            Debug.Console(1, device, "Switching Audio Input to : 'Hdmi1'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public static void SetAudioToHdmiInput2(this ICurrentAudioInput device)
        {
            Debug.Console(1, device, "Switching Audio Input to : 'Hdmi2'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public static void SetAudioToInputAnalog(this ICurrentAudioInput device)
        {
            Debug.Console(1, device, "Switching Audio Input to : 'Analog'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }

        public static void SetAudioToPrimaryStreamAudio(this ICurrentAudioInput device)
        {
            if (!device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching Audio Input to : 'PrimaryStream'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        public static void SetAudioToSecondaryStreamAudio(this ICurrentAudioInput device)
        {
            if (!device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching Audio Input to : 'SecondaryStream/DM NAX'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
        }

        public static void SetAudioToInputAutomatic(this ICurrentAudioInput device)
        {
            Debug.Console(1, device, "Switching Audio Input to : 'Automatic'");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
        }

        public static bool AudioInputIsLocal(INvxDeviceWithHardware device)
        {
            if (device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.AnalogAudio)
                return true;
            if (device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input1)
                return true;

            return device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input2;
        }
    }
}