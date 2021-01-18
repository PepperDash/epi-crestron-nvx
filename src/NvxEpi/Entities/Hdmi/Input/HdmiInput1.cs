using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Services.Feedback;

namespace NvxEpi.Entities.Hdmi.Input
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

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(Hdmi1HdcpCapabilityFeedback.GetFeedback(device.Hardware));
        }
    }
}