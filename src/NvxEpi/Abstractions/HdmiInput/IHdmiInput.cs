using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.HdmiInput
{
    public interface IHdmiInput : INvxHardware
    {
        ReadOnlyDictionary<uint, IntFeedback> HdcpCapability { get; }
        ReadOnlyDictionary<uint, BoolFeedback> SyncDetected { get; } 
    }

    public static class HdmiInputExtensions
    {
        public static void SetHdmi1HdcpCapability(this IHdmiInput device, ushort capability)
        {    
            try
            {
                if (device.Hardware.HdmiIn[1] == null)
                    throw new NotSupportedException("hdmi1");

                var capabilityToSet = (eHdcpCapabilityType)capability;
                Debug.Console(1, device, "Setting Hdmi1 Capability to '{0}", capabilityToSet.ToString());
                switch (capabilityToSet)
                {
                    case eHdcpCapabilityType.HdcpSupportOff:
                        device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.HdcpAutoSupport:
                        device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp1xSupport:
                        device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp2_2Support:
                        device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }  
        }

        public static void SetHdmi1HdcpCapability(this IHdmiInput device, eHdcpCapabilityType capability)
        {
            try
            {
                if (device.Hardware.HdmiIn[1] == null)
                    throw new NotSupportedException("hdmi1");

                Debug.Console(1, device, "Setting Hdmi1 Capability to '{0}", capability.ToString());
                device.Hardware.HdmiIn[1].HdcpCapability = capability; 
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
        }

        public static void SetHdmi2HdcpCapability(this IHdmiInput device, ushort capability)
        {
            try
            {
                if (device.Hardware.HdmiIn[2] == null)
                    throw new NotSupportedException("hdmi2");

                var capabilityToSet = (eHdcpCapabilityType)capability;

                Debug.Console(1, device, "Setting Hdmi2 Capability to '{0}", capabilityToSet.ToString());
                switch (capabilityToSet)
                {
                    case eHdcpCapabilityType.HdcpSupportOff:
                        device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.HdcpAutoSupport:
                        device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp1xSupport:
                        device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    case eHdcpCapabilityType.Hdcp2_2Support:
                        device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error setting Hdmi2 Capability : {0}", ex.Message);
            }  
        }

        public static void SetHdmi2HdcpCapability(this IHdmiInput device, eHdcpCapabilityType capability)
        {
            try
            {
                if (device.Hardware.HdmiIn[2] == null)
                    throw new NotSupportedException("hdmi1");

                Debug.Console(1, device, "Setting Hdmi2 Capability to '{0}", capability.ToString());
                device.Hardware.HdmiIn[2].HdcpCapability = capability;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error setting Hdmi1 Capability : {0}", ex.Message);
            }
        }
    }

}