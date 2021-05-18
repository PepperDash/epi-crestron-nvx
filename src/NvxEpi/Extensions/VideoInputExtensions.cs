using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class VideoInputExtensions
    {
        public static void SetVideoInput(this ICurrentVideoInput device, ushort input)
        {
            var inputToSwitch = (eSfpVideoSourceTypes)input;

            switch (inputToSwitch)
            {
                case eSfpVideoSourceTypes.Disable:
                    device.SetVideoToInputNone();
                    break;
                case eSfpVideoSourceTypes.Hdmi1:
                    device.SetVideoToHdmiInput1();
                    break;
                case eSfpVideoSourceTypes.Hdmi2:
                    device.SetVideoToHdmiInput2();
                    break;
                case eSfpVideoSourceTypes.Stream:
                    device.SetVideoToStream();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(input.ToString());
            }
        }

        public static void SetVideoToHdmiInput1(this ICurrentVideoInput device)
        {
            Debug.Console(1, device, "Switching Video Input to : 'Hdmi1'");
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
        }

        public static void SetVideoToHdmiInput2(this ICurrentVideoInput device)
        {
            Debug.Console(1, device, "Switching Video Input to : 'Hdmi2'");
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
        }

        public static void SetVideoToInputNone(this ICurrentVideoInput device)
        {
            Debug.Console(1, device, "Switching Video Input to : 'Disable'");
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Disable;
        }

        public static void SetVideoToStream(this ICurrentVideoInput device)
        {
            if (device.Hardware is DmNvxE3x || device.IsTransmitter)
                return;

            Debug.Console(1, device, "Switching Video Input to : 'Stream'");
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }

        public static void SetVideoToAutomatic(this ICurrentVideoInput device)
        {
            Debug.Console(1, device, "Switching Video Input to : 'Automatic'");
            device.Hardware.Control.EnableAutomaticInputRouting();
        }
    }
}