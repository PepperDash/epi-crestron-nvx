using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Services.Feedback;
using PepperDash.Core;

namespace NvxEpi.Entities.HdmiInput
{
    public class HdmiInput1 : HdmiInput, IHdmiInput1
    {
        public HdmiInput1(INvxHardware device) : base(device)
        {
            var nvx35X = Hardware as DmNvx35x;
            if (nvx35X != null)
            {
                _hdcpCapability.Add(1, Hdmi1HdcpCapabilityValueFeedback.GetFeedback(nvx35X));
                _syncDetected.Add(1, Hdmi1SyncDetectedFeedback.GetFeedback(nvx35X));
            }

            var nvxe3X = Hardware as DmNvxE3x;
            if (nvxe3X != null)
            {
                _hdcpCapability.Add(1, Hdmi1HdcpCapabilityValueFeedback.GetFeedback(nvxe3X));
                _syncDetected.Add(1, Hdmi1SyncDetectedFeedback.GetFeedback(nvxe3X));
            }

            Feedbacks.Add(_hdcpCapability[1]);
            Feedbacks.Add(_syncDetected[1]);
        }

        public void SetHdmi1HdcpCapability(int capability)
        {
            try
            {
                if (Hardware.HdmiIn[1] == null)
                    throw new NotSupportedException("hdmi1");

                var capabilityToSet = (eHdcpCapabilityType)capability;
                Debug.Console(1, this, "Setting Hdmi1 Capability to '{0}", capabilityToSet.ToString());

                switch (capabilityToSet)
                {
                    case eHdcpCapabilityType.HdcpSupportOff:
                        Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.HdcpAutoSupport:
                        Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp1xSupport:
                        Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp2_2Support:
                        Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }  
        }

        public void SetHdmi1HdcpCapability(eHdcpCapabilityType capability)
        {
            try
            {
                if (Hardware.HdmiIn[1] == null)
                    throw new NotSupportedException("hdmi1");

                Debug.Console(1, this, "Setting Hdmi1 Capability to '{0}", capability.ToString());
                Hardware.HdmiIn[1].HdcpCapability = capability;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
        }
    }
}