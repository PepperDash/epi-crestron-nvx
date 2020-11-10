using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IStream : INvxHardware
    {   
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }
    }
}