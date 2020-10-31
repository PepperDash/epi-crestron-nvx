using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface ICurrentStream : IStream
    {
        StringFeedback CurrentStreamName { get; }
        IntFeedback CurrentStreamId { get; }
    }
}