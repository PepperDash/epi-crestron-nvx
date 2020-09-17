using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface INaxAudioTx : INvxDevice
    {
        StringFeedback NaxAudioTxAddress { get; }
        BoolFeedback IsStreamingNaxTx { get; }
    }
}