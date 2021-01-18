using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface INaxAudioStream : INaxAudioTxWithHardware, INaxAudioRxWithHardware
    {
        StringFeedback CurrentNaxInput { get; }
        IntFeedback CurrentNaxInputValue { get; }
    }
}