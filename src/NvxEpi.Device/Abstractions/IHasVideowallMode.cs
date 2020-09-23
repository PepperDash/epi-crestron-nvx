using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasVideowallMode : IHasHdmiOutput
    {
        IntFeedback VideowallMode { get; }
    }
}