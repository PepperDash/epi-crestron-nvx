using Crestron.SimplSharpPro.DM.Streaming;
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
            var capability = (device.Hardware is DmNvxE760x) 
                ? DmHdcpCapabilityValueFeedback.GetFeedback(device.Hardware) 
                : Hdmi1HdcpCapabilityValueFeedback.GetFeedback(device.Hardware);

            _capability.Add(1, capability);

            var sync = (device.Hardware is DmNvxE760x) 
                ? DmSyncDetectedFeedback.GetFeedback(device.Hardware)
                : Hdmi1SyncDetectedFeedback.GetFeedback(device.Hardware);

            _sync.Add(1, sync);

            //TODO
            var inputResolution = new StringFeedback(() => string.Empty);
            _currentResolution.Add(1, inputResolution);

            var capabilityString = Hdmi1HdcpCapabilityFeedback.GetFeedback(device.Hardware);

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(inputResolution);
            Feedbacks.Add(capabilityString);
        }
    }
}
