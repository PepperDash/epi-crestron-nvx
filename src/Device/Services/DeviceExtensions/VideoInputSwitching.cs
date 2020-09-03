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
        public static void SetInput(this IVideoInputSwitcher device, VideoInputEnum input)
        {
            if (device.IsTransmitter.BoolValue)
                device.SetVideoInputForTransmitter(input);
            else
                device.SetVideoInputForReceiver(input);
        }

        private static void SetVideoInputForTransmitter(this IHardware device, VideoInputEnum input)
        {
            if (input == VideoInputEnum.Stream)
                return;

            device.Hardware.Control.VideoSource = (eSfpVideoSourceTypes) input.Value;
        }

        private static void SetVideoInputForReceiver(this IHardware device, VideoInputEnum input)
        {
            device.Hardware.Control.VideoSource = (eSfpVideoSourceTypes)input.Value;
        }
    }
}