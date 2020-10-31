using System;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Services.Feedback;
using PepperDash.Core;

namespace NvxEpi.Entities.HdmiInput
{
    public class HdmiInput2 : HdmiInput1, IHdmiInput2
    {
        public HdmiInput2(INvxHardware device)
            : base(device)
        {
            var nvx35X = Hardware as DmNvx35x;
            if (nvx35X != null)
            {
                _hdcpCapability.Add(2, Hdmi2HdcpCapabilityValueFeedback.GetFeedback(nvx35X));
                _syncDetected.Add(2, Hdmi2SyncDetectedFeedback.GetFeedback(nvx35X));
            }

            Feedbacks.Add(_hdcpCapability[2]);
            Feedbacks.Add(_syncDetected[2]);
        }

        public void SetHdmi2HdcpCapability(int capability)
        {
            try
            {
                if (Hardware.HdmiIn[2] == null)
                    throw new NotSupportedException("hdmi1");

                var capabilityToSet = (eHdcpCapabilityType) capability;
                Debug.Console(1, this, "Setting Hdmi1 Capability to '{0}", capabilityToSet.ToString());

                switch (capabilityToSet)
                {
                    case eHdcpCapabilityType.HdcpSupportOff:
                        Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.HdcpAutoSupport:
                        Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp1xSupport:
                        Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp2_2Support:
                        Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
        }

        public void SetHdmi2HdcpCapability(eHdcpCapabilityType capability)
        {
            try
            {
                if (Hardware.HdmiIn[2] == null)
                    throw new NotSupportedException("hdmi1");

                Debug.Console(1, this, "Setting Hdmi1 Capability to '{0}", capability.ToString());
                Hardware.HdmiIn[2].HdcpCapability = capability;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, this, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
        }
    }
}