using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IVideoInputSwitcher : INvxDevice
    {
        StringFeedback VideoInputName { get; }
        IntFeedback VideoInputValue { get; }
    }
}