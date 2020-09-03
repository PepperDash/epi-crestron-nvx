using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IVideowallMode : IHdmiOutput
    {
        IntFeedback VideowallMode { get; }
    }
}