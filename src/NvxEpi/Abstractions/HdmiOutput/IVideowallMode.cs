using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IVideowallMode : IHdmiOutputWithAspect
    {
        IntFeedback VideowallMode { get; }
    }
}