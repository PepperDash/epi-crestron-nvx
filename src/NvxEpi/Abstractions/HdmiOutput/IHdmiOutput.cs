using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiOutput
{
    public interface IHdmiOutput : INvxHardware
    {
        BoolFeedback DisabledByHdcp { get; }
        IntFeedback HorizontalResolution { get; }
    }
}