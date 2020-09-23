using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface INaxAudioRx : INvxHardware, IDeviceId
    {
        StringFeedback NaxAudioRxAddress { get; }
        BoolFeedback IsStreamingNaxRx { get; }
    }

    
}