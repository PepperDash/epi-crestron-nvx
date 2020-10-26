using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface INaxAudioStream : INaxAudioTx, INaxAudioRx, INvxDevice
    {
        StringFeedback CurrentNaxInput { get; }
        IntFeedback CurrentNaxInputValue { get; }
    }
}