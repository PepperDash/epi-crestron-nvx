using NvxEpi.Abstractions;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Hdmi.Input
{
    public class HdmiInput1 : HdmiInputBase
    {
        public HdmiInput1(INvxDeviceWithHardware device)
            : base(device)
        {
            var capability = Hdmi1HdcpCapabilityValueFeedback.GetFeedback(device.Hardware);
            _capability.Add(1, capability);

            var sync = Hdmi1SyncDetectedFeedback.GetFeedback(device.Hardware);
            _sync.Add(1, sync);

            //TODO
            var inputResolution = new StringFeedback(() => string.Empty);
            _currentResolution.Add(1, inputResolution);

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(inputResolution);
        }
    }
}
