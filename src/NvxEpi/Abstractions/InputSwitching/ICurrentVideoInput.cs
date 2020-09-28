using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentVideoInput : INvxDevice
    {
        StringFeedback CurrentVideoInput { get; }
        IntFeedback CurrentVideoInputValue { get; }
    }
}