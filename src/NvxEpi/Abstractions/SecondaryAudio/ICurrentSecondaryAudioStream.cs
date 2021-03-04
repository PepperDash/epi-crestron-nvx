using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ICurrentSecondaryAudioStream : ISecondardyAudioStreamWithHardware
    {
        StringFeedback CurrentSecondaryAudioStreamName { get; }
        IntFeedback CurrentSecondaryAudioStreamId { get; }
    }
}