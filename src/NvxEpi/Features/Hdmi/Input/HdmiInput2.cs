using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Input
{
    public class HdmiInput2 : HdmiInput1
    {
        public HdmiInput2(INvxDeviceWithHardware device) : base(device)
        {
            var capability = Hdmi2HdcpCapabilityValueFeedback.GetFeedback(device.Hardware);
            _capability.Add(2, capability);

            var sync = Hdmi2SyncDetectedFeedback.GetFeedback(device.Hardware);
            _sync.Add(2, sync);

            //TODO
            var inputResolution = new StringFeedback(() => string.Empty);
            _currentResolution.Add(2, inputResolution);

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(inputResolution);
        }
    }
}