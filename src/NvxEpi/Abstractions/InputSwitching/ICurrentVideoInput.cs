using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentVideoInput : INvxDeviceWithHardware
    {
        StringFeedback CurrentVideoInput { get; }
        IntFeedback CurrentVideoInputValue { get; }
    }
}