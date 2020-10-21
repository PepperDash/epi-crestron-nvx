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
            device.Control.EnableAutomaticInitiation();
            device.SecondaryAudio.EnableAutomaticInitiation();
            device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();
            device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
            device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;

            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;
        }

        public static void SetRxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Receiver;
            device.Control.EnableAutomaticInitiation();
            device.SecondaryAudio.EnableAutomaticInitiation();
            device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();
            device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
            device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
            device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
        }
    }
}