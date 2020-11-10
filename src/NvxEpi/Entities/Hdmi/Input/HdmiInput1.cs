using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Input
{
    public class HdmiInput1 : HdmiInputBase
    {
        private HdmiInput1(INvxHardware device) : base(device)
        {
            
        }

        public HdmiInput1(INvx35XHardware device)
            : this(device as INvxHardware)
        {
            var capability = Hdmi1HdcpCapabilityValueFeedback.GetFeedback(device.Hardware);
            _capability.Add(1, capability);

            var sync = Hdmi1SyncDetectedFeedback.GetFeedback(device.Hardware);
            _sync.Add(1, sync);

            Feedbacks.Add(capability);
            Feedbacks.Add(sync);
            Feedbacks.Add(Hdmi1HdcpCapabilityFeedback.GetFeedback(device.Hardware));
        }

        public HdmiInput1(INvxE3XHardware device)
            : this(device as INvxHardware)
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