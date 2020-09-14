using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Device
{
    public class CurrentVideoInput : IHasCurrentVideoInput
    {
        private readonly INvxDevice _device;

        public CurrentVideoInput(INvxDevice device)
        {
            _device = device;

            Initialize();
        }

        public StringFeedback VideoInputName { get; private set; }
        public IntFeedback VideoInputValue { get; private set; }

        private void Initialize()
        {
            VideoInputName = _device.Hardware.GetVideoInputFeedback();
            VideoInputValue = _device.Hardware.GetVideoInputValueFeedback();

            _device.Feedbacks.AddRange(new Feedback[]
            {
                VideoInputName,
                VideoInputValue
            });
        }
    }
}