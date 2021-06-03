using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IHdmiOutputWithAspect : IHdmiOutput
    {
        IntFeedback VideoAspectRatioMode { get; }
    }
}