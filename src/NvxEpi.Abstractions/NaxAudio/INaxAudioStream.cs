using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface INaxAudioStream : INaxAudioTx, INaxAudioRx
    {
        StringFeedback CurrentNaxInput { get; }
        IntFeedback CurrentNaxInputValue { get; }
    }
}