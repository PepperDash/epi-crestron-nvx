using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasCurrentAudioInput : INvxDevice
    {
        StringFeedback AudioInputName { get; }
        IntFeedback AudioInputValue { get; }
    }
}