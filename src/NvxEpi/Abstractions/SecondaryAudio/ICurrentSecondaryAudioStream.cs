using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ICurrentSecondaryAudioStream : ISecondaryAudioStream
    {
        StringFeedback CurrentSecondaryAudioStreamName { get; }
        IntFeedback CurrentSecondaryAudioStreamId { get; }
    }
}