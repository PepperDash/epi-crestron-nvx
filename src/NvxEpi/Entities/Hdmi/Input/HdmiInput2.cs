using NvxEpi.Abstractions.Hardware;
using NvxEpi.Services.Feedback;

namespace NvxEpi.Entities.Hdmi.Input
{
    public class HdmiInput2 : HdmiInput1
    {
        public HdmiInput2(INvx35XHardware device) : base(device)
        {
            var capability = Hdmi2HdcpCapabilityValueFeedback.GetFeedback(device.Hardware);
            _capability.Add(2, capability);

            var sync = Hdmi2SyncDetectedFeedback.GetFeedback(device.Hardware);
            _sync.Add(2, sync);

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(Hdmi2HdcpCapabilityFeedback.GetFeedback(device.Hardware));
        }
    }
}