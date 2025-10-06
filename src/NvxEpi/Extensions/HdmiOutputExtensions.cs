using System;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.HdmiOutput;
using PepperDash.Core.Logging;

namespace NvxEpi.Extensions;

public static class HdmiOutputExtensions
{
    public static void SetVideoAspectRatioMode(this IVideowallMode device, ushort mode)
    {
        try
        {
            if (device.Hardware.HdmiOut == null)
                throw new NotSupportedException("HdmiOut");

            var modeToSet = (eAspectRatioMode)mode;


            device.LogDebug("Setting Video Aspect Ratio to '{mode}'", modeToSet.ToString());
            device.Hardware.HdmiOut.VideoAttributes.AspectRatioMode = modeToSet;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            device.LogError("Error setting Aspect Ratio : {0}", ex.Message);
        }
        catch (NotSupportedException ex)
        {
            device.LogError("Error setting Aspect Ratio : {0}", ex.Message);
        }
        catch (Exception ex)
        {
            device.LogError("Error setting Aspect Ratio Capability : {0}", ex.Message);
            device.LogDebug(ex, "Stack Trace: ");
        }
    }

    public static void SetVideowallMode(this IVideowallMode device, ushort value)
    {
        if (device.IsTransmitter)
            return;

        device.LogDebug("Setting videowall mode to : {mode}", value);
        if (device.Hardware.HdmiOut != null)
            device.Hardware.HdmiOut.VideoWallMode.UShortValue = value;
    }
}