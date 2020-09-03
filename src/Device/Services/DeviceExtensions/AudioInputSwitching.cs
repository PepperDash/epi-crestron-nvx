using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class AudioInputSwitching
    {
        public static void SetInput(this IAudioInputSwitcher device, AudioInputEnum input)
        {
            if (device.IsTransmitter.BoolValue)
                device.SetAudioInputForTransmitter(input);
            else
                device.SetAudioInputForReceiver(input);
        }

        private static void SetAudioInputForTransmitter(this IHardware device, AudioInputEnum input)
        {
            if (input == AudioInputEnum.PrimaryStream || input == AudioInputEnum.SecondaryStream)
                return;

            device.Hardware.Control.AudioSource = (DmNvxControl.eAudioSource) input.Value;
        }

        private static void SetAudioInputForReceiver(this IHardware device, AudioInputEnum input)
        {
            device.Hardware.Control.AudioSource = (DmNvxControl.eAudioSource) input.Value;
        }
    }
}