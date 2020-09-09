using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class AudioInputSwitching
    {
        public static void SetInput(this IAudioInputSwitcher device, DmNvxControl.eAudioSource input)
        {
            if (device.IsTransmitter.BoolValue)
                device.SetAudioInputForTransmitter(input);
            else
                device.SetAudioInputForReceiver(input);
        }

        private static void SetAudioInputForTransmitter(this IHardware device, DmNvxControl.eAudioSource input)
        {
            if (input == DmNvxControl.eAudioSource.PrimaryStreamAudio || input == DmNvxControl.eAudioSource.SecondaryStreamAudio)
                return;

            device.Hardware.Control.AudioSource = input;
        }

        private static void SetAudioInputForReceiver(this IHardware device, DmNvxControl.eAudioSource input)
        {
            device.Hardware.Control.AudioSource = input;
        }
    }
}