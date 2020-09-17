using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using PepperDash.Core;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxHardware : ITransmitterReceiver, IKeyed
    {
        DmNvxBaseClass Hardware { get; }
    }

    public static class NvxHardwareExtensions
    {
        public static void SetVideoToInputNone(this INvxHardware device)
        {
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Disable;
        }

        public static void SetAudioToInputAutomatic(this INvx35XHardware device)
        {
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
        }
    }
}