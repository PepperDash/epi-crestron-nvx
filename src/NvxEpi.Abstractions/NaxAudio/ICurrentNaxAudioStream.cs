using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.NaxAudio
{
    public interface ICurrentNaxAudioStream : INaxAudioStream
    {
        StringFeedback CurrentNaxAudioStreamName { get; }
        IntFeedback CurrentNaxAudioStreamId { get; }
    }
}