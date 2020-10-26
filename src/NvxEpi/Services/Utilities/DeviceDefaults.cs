using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Entities.Config;
using PepperDash.Core;

namespace NvxEpi.Services.Utilities
{
    public static class DeviceDefaults
    {
        public static void SetTxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Transmitter;
            
            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;
        }

        public static void SetRxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Receiver;
        }
    }
}