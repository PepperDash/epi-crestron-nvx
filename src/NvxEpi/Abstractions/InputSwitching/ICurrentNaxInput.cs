using NvxEpi.Abstractions.Dante;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentNaxInput : INvxDeviceWithHardware
    {
        StringFeedback CurrentNaxInput { get; }
        IntFeedback CurrentNaxInputValue { get; }
    }
}