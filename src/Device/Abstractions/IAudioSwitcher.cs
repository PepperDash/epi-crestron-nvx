using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IAudioInputSwitcher : INvxDevice
    {
        StringFeedback AudioInputName { get; }
        IntFeedback AudioInputValue { get; }

        void SetInputToFollowVideo();
        void SetInputToStream();
        void SetInputToHdmi1();
        void SetInputToHdmi2();
    }
}