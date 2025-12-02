using System;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.UsbcInput;
using PepperDash.Core;

namespace NvxEpi.Extensions;

public static class UsbcInputExtensions
{        
    public static void SetUsbc1HdcpCapability(this INvxUsbcInput device, ushort capability)
    {
        try
        {
            eHdcpCapabilityType capabilityToSet;

            if (device.Hardware.UsbcIn != null && device.Hardware.UsbcIn[1] != null)
            {
                capabilityToSet = (eHdcpCapabilityType) capability;
                Debug.LogInformation(device, "Setting Usbc1 Capability to '{0}'", capabilityToSet.ToString());
                device.Hardware.UsbcIn[1].HdcpCapability = capabilityToSet;
            }
            else
            {
                throw new NotSupportedException("Usbc1");
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(device, "Error setting Usbc1 Capability : {0}", ex.Message);
        }
    }

    public static void SetUsbc1HdcpCapability(this INvxUsbcInput device, eHdcpCapabilityType capability)
    {
        try
        {
            if (device.Hardware.UsbcIn[1] == null)
                throw new NotSupportedException("usbc1");

            Debug.LogInformation(device, "Setting Usbc1 Capability to '{0}'", capability.ToString());
            device.Hardware.UsbcIn[1].HdcpCapability = capability;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(device, "Error setting Usbc1 Capability : {0}", ex.Message);
        }
    }

    public static void SetUsbc2HdcpCapability(this INvxUsbcInput device, ushort capability)
    {
        try
        {
            if (device.Hardware.UsbcIn[2] == null)
                throw new NotSupportedException("usbc2");

            var capabilityToSet = (eHdcpCapabilityType) capability;

            Debug.LogInformation(device, "Setting Usbc2 Capability to '{0}'", capabilityToSet.ToString());
            device.Hardware.UsbcIn[2].HdcpCapability = capabilityToSet;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(device, "Error setting Usbc2 Capability : {0}", ex.Message);
        }
    }

    public static void SetUsbc2HdcpCapability(this INvxUsbcInput device, eHdcpCapabilityType capability)
    {
        try
        {
            if (device.Hardware.UsbcIn[2] == null)
                throw new NotSupportedException("usbc2");

            Debug.LogInformation(device, "Setting Usbc2 Capability to '{0}'", capability.ToString());
            device.Hardware.UsbcIn[2].HdcpCapability = capability;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            Debug.LogWarning(device, "Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(device, "Error setting Usbc2 Capability : {0}", ex.Message);
        }
    }
}