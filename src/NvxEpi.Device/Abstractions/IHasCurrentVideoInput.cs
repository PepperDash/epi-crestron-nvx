using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasCurrentVideoInput : INvxDevice
    {
        StringFeedback VideoInputName { get; }
        IntFeedback VideoInputValue { get; }
    }
}