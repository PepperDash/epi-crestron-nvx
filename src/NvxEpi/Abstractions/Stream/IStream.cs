using NvxEpi.Abstractions.Device;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IStream : INvxDevice, IMulticastAddress
    {   
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }
        StringFeedback StreamUrl { get; }
    }
}