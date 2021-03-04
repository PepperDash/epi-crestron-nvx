using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IMulticastAddress
    {
        StringFeedback MulticastAddress { get; }
    }
}