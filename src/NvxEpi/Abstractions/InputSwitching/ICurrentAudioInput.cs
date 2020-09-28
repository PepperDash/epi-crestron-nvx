using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentAudioInput : INvxDevice
    {
        StringFeedback CurrentAudioInput { get; }
        IntFeedback CurrentAudioInputValue { get; }
    }
}