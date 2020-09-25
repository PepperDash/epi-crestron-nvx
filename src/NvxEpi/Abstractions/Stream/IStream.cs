using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IStream : INvxDevice
    {
        StringFeedback StreamUrl { get; }
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }
    }
}