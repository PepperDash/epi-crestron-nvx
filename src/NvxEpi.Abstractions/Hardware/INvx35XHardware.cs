using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvx35XHardware : INvxHardware
    {
        new DmNvx35x Hardware { get; }
    }

    public static class Nvx35XHardwareExtensions
    {
        public static void SetVideoToHdmiInput1(this INvx35XHardware device)
        {
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
        }

        public static void SetVideoToHdmiInput2(this INvx35XHardware device)
        {
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
        }

        public static void SetAudioToHdmiInput1(this INvx35XHardware device)
        {
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
        }

        public static void SetAudioToHdmiInput2(this INvx35XHardware device)
        {
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
        }

        public static void SetAudioToInputAnalog(this INvx35XHardware device)
        {
            if (!device.IsTransmiter)
                return;

            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio;
        }
    }
}