using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface ICurrentStream : IStreamWithHardware
    {
        StringFeedback CurrentStreamName { get; }
        IntFeedback CurrentStreamId { get; }
    }
}