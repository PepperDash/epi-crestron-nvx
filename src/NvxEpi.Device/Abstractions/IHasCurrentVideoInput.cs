using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasCurrentVideoInput
    {
        StringFeedback VideoInputName { get; }
        IntFeedback VideoInputValue { get; }
    }
}