using System;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.HdmiOutput;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class HdmiOutputExtensions
    {
        public static void SetVideoAspectRatioMode(this IVideowallMode device, ushort mode)
        {
            try
            {
                if (device.Hardware.HdmiOut == null)
                    throw new NotSupportedException("HdmiOut");

                var modeToSet = (eAspectRatioMode) mode;


                Debug.Console(1, device, "Setting Video Aspect Ratio to '{0}'", modeToSet.ToString());
                device.Hardware.HdmiOut.VideoAttributes.AspectRatioMode = modeToSet;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Debug.Console(1, device, "Error setting Aspect Ratio : {0}", ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Debug.Console(1, device, "Error setting Aspect Ratio : {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error setting Aspect Ratio Capability : {0}", ex.Message);
            }
        }

        public static void SetVideowallMode(this IVideowallMode device, ushort value)
        {
            if (device.IsTransmitter)
                return;

            Debug.Console(1, device, "Setting videowall mode to : '{0}'", value);
            if (device.Hardware.HdmiOut != null)
                device.Hardware.HdmiOut.VideoWallMode.UShortValue = value;
        }
    }
}