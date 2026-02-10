using System;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.UsbcInput;
using PepperDash.Core.Logging;

namespace NvxEpi.Extensions;

public static class UsbcInputExtensions
{
    public static void SetUsbc1HdcpCapability(this IUsbcInput device, ushort capability)
    {
        throw new NotImplementedException("Setting Usbc1 Hdcp Capability is not supported at this time.");
    }

    public static void SetUsbc1HdcpCapability(this IUsbcInput device, eHdcpCapabilityType capability)
    {
        throw new NotImplementedException("Setting Usbc1 Hdcp Capability is not supported at this time.");
    }

    public static void SetUsbc2HdcpCapability(this IUsbcInput device, ushort capability)
    {
        throw new NotImplementedException("Setting Usbc2 Hdcp Capability is not supported at this time.");
    }

    public static void SetUsbc2HdcpCapability(this IUsbcInput device, eHdcpCapabilityType capability)
    {
        throw new NotImplementedException("Setting Usbc2 Hdcp Capability is not supported at this time.");
    }

    /*
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(25,33): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(27,38): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(27,72): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(31,33): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(57,33): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(61,29): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(82,33): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(88,29): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(109,33): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    /epi-crestron-nvx/src/NvxEpi/Extensions/UsbcInputExtensions.cs(113,29): error CS1061: 'DmNvxBaseClass' does not contain a definition for 'UsbcIn' and no accessible extension method 'UsbcIn' accepting a first argument of type 'DmNvxBaseClass' could be found (are you missing a using directive or an assembly reference?)
    */

    /*
    public static void SetUsbc1HdcpCapability(this IUsbcInput device, ushort capability)
    {
        try
        {
            eHdcpCapabilityType capabilityToSet;

            if (device.Hardware is DmNvxE760x)
            {
                device.LogDebug("DmNvxE760x detected - Usbc1 Hdcp Capability setting is not supported.");
                return;
            }
            else if (device.Hardware is DmNvx38x)
            {
                capabilityToSet = (eHdcpCapabilityType)capability;
                device.LogDebug("Setting Usbc1 Capability to '{0}'", capabilityToSet.ToString());
                device.UsbcIn[1].HdcpCapability = capabilityToSet;
            }
            else if (device.Hardware.UsbcIn != null && device.Hardware.UsbcIn[1] != null)
            {
                capabilityToSet = (eHdcpCapabilityType)capability;
                device.LogDebug("Setting Usbc1 Capability to '{0}'", capabilityToSet.ToString());
                device.Hardware.UsbcIn[1].HdcpCapability = capabilityToSet;
            }
            else
            {
                throw new NotSupportedException("usbc1");
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            device.LogError("Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            device.LogError("Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            device.LogError("Error setting Usbc1 Capability : {0}", ex.Message);
            device.LogDebug(ex, "Stack Trace: ");
        }
    }

    public static void SetUsbc1HdcpCapability(this IUsbcInput device, eHdcpCapabilityType capability)
    {
        try
        {
            if (device.Hardware.UsbcIn[1] == null)
                throw new NotSupportedException("usbc1");

            device.LogDebug("Setting Usbc1 Capability to '{0}'", capability.ToString());
            device.Hardware.UsbcIn[1].HdcpCapability = capability;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            device.LogError("Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            device.LogError("Error setting Usbc1 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            device.LogError("Error setting Usbc1 Capability : {0}", ex.Message);
            device.LogDebug(ex, "Stack Trace: ");
        }
    }

    public static void SetUsbc2HdcpCapability(this IUsbcInput device, ushort capability)
    {
        try
        {
            if (device.Hardware.UsbcIn[2] == null)
                throw new NotSupportedException("usbc2");

            var capabilityToSet = (eHdcpCapabilityType)capability;

            device.LogDebug("Setting Usbc2 Capability to '{0}'", capabilityToSet.ToString());
            device.Hardware.UsbcIn[2].HdcpCapability = capabilityToSet;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            device.LogError("Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            device.LogError("Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            device.LogError("Error setting Usbc2 Capability : {0}", ex.Message);
            device.LogDebug(ex, "Stack Trace: ");
        }
    }

    public static void SetUsbc2HdcpCapability(this IUsbcInput device, eHdcpCapabilityType capability)
    {
        try
        {
            if (device.Hardware.UsbcIn[2] == null)
                throw new NotSupportedException("usbc2");

            device.LogDebug("Setting Usbc2 Capability to '{0}'", capability.ToString());
            device.Hardware.UsbcIn[2].HdcpCapability = capability;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            device.LogError("Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            device.LogError("Error setting Usbc2 Capability : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            device.LogError("Error setting Usbc2 Capability : {0}", ex.Message);
            device.LogDebug(ex, "Stack Trace: ");
        }
    }
    */
}