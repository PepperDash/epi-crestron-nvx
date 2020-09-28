using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface ICurrentStream : IStream, IHasFeedback
    {
        StringFeedback CurrentStreamName { get; }
        IntFeedback CurrentStreamId { get; }
    }
}