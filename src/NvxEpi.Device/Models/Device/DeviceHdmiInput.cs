using System;
using System.Globalization;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Device
{
    public class DeviceHdmiInput
    {
        private readonly INvxDevice _device;

        public DeviceHdmiInput(int index, INvxDevice device)
        {
            _device = device;
            
            Initialize(index);
        }

        private void Initialize(int index)
        {
            if (_device.Hardware.HdmiIn == null || _device.Hardware.HdmiIn[(uint)index] == null)
                throw new NotSupportedException("Hdmi In " + index);

            Input = _device.Hardware.HdmiIn[(uint)index];
            switch (index)
            {
                case 1:
                    SyncDetected = _device.Hardware.GetHdmiIn1SyncDetectedFeedback();
                    CapabilityName = _device.Hardware.GetHdmiIn1HdcpCapabilityFeedback();
                    CapabilityValue = _device.Hardware.GetHdmiIn1HdcpCapabilityValueFeedback();
                    break;
                case 2:
                    SyncDetected = _device.Hardware.GetHdmiIn2SyncDetectedFeedback();
                    CapabilityName = _device.Hardware.GetHdmiIn2HdcpCapabilityFeedback();
                    CapabilityValue = _device.Hardware.GetHdmiIn2HdcpCapabilityValueFeedback();
                    break;
                default:
                    throw new NotSupportedException(index.ToString(CultureInfo.InvariantCulture));
            }

            _device.Feedbacks.Add(SyncDetected);
            _device.Feedbacks.Add(CapabilityName);
            _device.Feedbacks.Add(CapabilityValue);
        }

        public HdmiInWithColorSpaceMode Input { get; private set; }
        public BoolFeedback SyncDetected { get; private set; }
        public StringFeedback CapabilityName { get; private set; }
        public IntFeedback CapabilityValue { get; private set; }
    }
}