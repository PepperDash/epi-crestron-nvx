using System;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.HdmiOutput;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class HdmiOutputExtensions
    {
        public static void SetVideoAspectRatioMode(this IHdmiOutput device, ushort mode)
        {
            try
            {
                if (device.Hardware.HdmiOut == null)
                    throw new NotSupportedException("HdmiOut");



                var modeToSet = (eAspectRatioMode)mode;

                Debug.Console(1, device, "Setting Video Aspect Ratio to '{0}", mode.ToString());
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

    }
}