using NvxEpi.Abstractions.Device;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IStream : INvxDevice, IStreamUrl, IMulticastAddress
    {   
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }
    }
}