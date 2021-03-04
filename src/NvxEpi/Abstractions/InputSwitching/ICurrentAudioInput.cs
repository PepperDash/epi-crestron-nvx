using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface ICurrentAudioInput : INvxDeviceWithHardware
    {
        StringFeedback CurrentAudioInput { get; }
        IntFeedback CurrentAudioInputValue { get; }
    }
}