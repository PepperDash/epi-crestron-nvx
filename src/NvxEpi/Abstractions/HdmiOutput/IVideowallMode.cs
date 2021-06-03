using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IVideowallMode : IHdmiOutput
    {
        IntFeedback VideowallMode { get; }
        IntFeedback VideoAspectRatioMode { get; }
    }
}