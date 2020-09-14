using System;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Device
{
    public class DeviceHdmiOutput : IHasHdmiOutput
    {
        private readonly INvxDevice _device;

        public DeviceHdmiOutput(INvxDevice device)
        {
            _device = device;           
            Initialize();
        }

        private void Initialize()
        {
            if (_device.Hardware.HdmiOut == null)
                throw new NotSupportedException("Hdmi Output");

            HorizontalResolution = _device.Hardware.GetHorizontalResolutionFeedback();
            DisabledByHdcp = _device.Hardware.GetHdmiOutputDisabledFeedback();

            _device.Feedbacks.Add(HorizontalResolution);
            _device.Feedbacks.Add(DisabledByHdcp);
        }

        public IntFeedback HorizontalResolution { get; private set; }
        public BoolFeedback DisabledByHdcp { get; private set; }
    }
}