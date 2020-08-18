using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class VideoInput
    {
        public static StringFeedback GetVideoInputFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.VideoInputName.ToString(),
                () => device.Control.ActiveVideoSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static IntFeedback GetVideoInputValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.VideoInputValue.ToString(),
                () => (int) device.Control.ActiveVideoSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}