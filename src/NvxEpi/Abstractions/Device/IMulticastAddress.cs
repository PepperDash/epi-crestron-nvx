using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Device
{
    public interface IMulticastAddress
    {
        StringFeedback MulticastAddress { get; }
    }
}