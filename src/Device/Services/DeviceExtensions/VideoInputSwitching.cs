using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class VideoInputSwitching
    {
        public static void SetInput(this IVideoInputSwitcher device, eSfpVideoSourceTypes input)
        {
            if (device.IsTransmitter.BoolValue)
                device.SetVideoInputForTransmitter(input);
            else
                device.SetVideoInputForReceiver(input);
        }

        private static void SetVideoInputForTransmitter(this IHardware device, eSfpVideoSourceTypes input)
        {
            if (input == eSfpVideoSourceTypes.Stream)
                return;

            device.Hardware.Control.VideoSource = input;
        }

        private static void SetVideoInputForReceiver(this IHardware device, eSfpVideoSourceTypes input)
        {
            device.Hardware.Control.VideoSource = input;
        }
    }
}