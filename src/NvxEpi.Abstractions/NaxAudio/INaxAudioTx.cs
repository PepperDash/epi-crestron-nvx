using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface INaxAudioTx : INvxHardware, IDeviceId
    {
        StringFeedback NaxAudioTxAddress { get; }
        BoolFeedback IsStreamingNaxTx { get; }
    }
}