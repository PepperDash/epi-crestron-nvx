using System;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class HdmiInputExtensions
    {        
        public static void SetHdmi1HdcpCapability(this IHdmiInput device, ushort capability)
        {
            try
            {
                eHdcpCapabilityType capabilityToSet;

                if (device.Hardware is DmNvxE760x)
                {
                    capabilityToSet = (eHdcpCapabilityType)capability;
                    Debug.Console(1, device, "Setting Hdmi1 Capability to '{0}'", capabilityToSet.ToString());
                    var hardware = device.Hardware as DmNvxE760x;
                    hardware.DmIn.HdcpCapability = capabilityToSet;
                }
                else if (device.Hardware.HdmiIn != null && device.Hardware.HdmiIn[1] != null)
                {
                    capabilityToSet = (eHdcpCapabilityType) capability;
                    Debug.Console(1, device, "Setting Hdmi1 Capability to '{0}'", capabilityToSet.ToString());
                    device.Hardware.HdmiIn[1].HdcpCapability = capabilityToSet;
                }
                else
                {
                    throw new NotSupportedException("hdmi1");
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

                Debug.Console(1, device, "Setting Hdmi1 Capability to '{0}'", capability.ToString());
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

                var capabilityToSet = (eHdcpCapabilityType) capability;

                Debug.Console(1, device, "Setting Hdmi2 Capability to '{0}'", capabilityToSet.ToString());
                device.Hardware.HdmiIn[2].HdcpCapability = capabilityToSet;
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

                Debug.Console(1, device, "Setting Hdmi2 Capability to '{0}'", capability.ToString());
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