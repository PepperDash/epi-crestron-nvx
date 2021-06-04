using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Features.Config;
using PepperDash.Core;

namespace NvxEpi.Services.Utilities
{
    public static class DeviceDefaults
    {
        public static void SetTxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device is DmNvx35x || device is DmNvx36x)
            {
                device.Control.DeviceMode = eDeviceMode.Transmitter;
            }

            device.Control.EnableAutomaticInitiation();
            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
            {
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;
            }
            device.SetAudioDefaults(props);
        }

        public static void SetRxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device is DmNvx35x || device is DmNvx36x)
            {
                device.Control.DeviceMode = eDeviceMode.Receiver;
            }

            device.Control.EnableAutomaticInitiation();
            device.SetAudioDefaults(props);
        }

        public static void SetAudioDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
                device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.DmNaxRouting.DmNaxTransmit.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
                else
                {
                    device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                }
            }
        }
    }
}