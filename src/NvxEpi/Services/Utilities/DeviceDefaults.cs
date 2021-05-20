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
            if (device is DmNvx35x)
                (device as DmNvx35x).SetTxDefaults(props);

            else if (device is DmNvx36x)
                (device as DmNvx36x).SetTxDefaults(props);

            else if (device is DmNvxE3x)
                (device as DmNvxE3x).SetDefaults(props);
        }

        public static void SetRxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device is DmNvx35x)
                (device as DmNvx35x).SetRxDefaults(props);

            else if (device is DmNvx36x)
                (device as DmNvx36x).SetRxDefaults(props);

            else if (device is DmNvxD3x)
                (device as DmNvxD3x).SetDefaults(props);
        }

        public static void SetTxDefaults(this DmNvx35x device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Transmitter;
            device.Control.EnableAutomaticInitiation();

            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;


            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.DmNaxRouting.DmNaxTransmit.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
            }
        }

        public static void SetRxDefaults(this DmNvx35x device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Receiver;
            device.Control.EnableAutomaticInitiation();

            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
            }
        }

        public static void SetTxDefaults(this DmNvx36x device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Transmitter;
            device.Control.EnableAutomaticInitiation();

            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;

            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.DmNaxRouting.DmNaxTransmit.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
            }
        }

        public static void SetRxDefaults(this DmNvx36x device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Receiver;
            device.Control.EnableAutomaticInitiation();

            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
            }
        }

        public static void SetDefaults(this DmNvxE3x device, NvxDeviceProperties props)
        {
            device.Control.EnableAutomaticInitiation();
            device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;

            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;
        }

        public static void SetDefaults(this DmNvxD3x device, NvxDeviceProperties props)
        {
            device.Control.EnableAutomaticInitiation();
            device.SecondaryAudio.EnableAutomaticInitiation();
            device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
        }

        public static void SetAudioDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device is DmNvx35x)
                (device as DmNvx35x).SetAudioDefaults(props);

            else if (device is DmNvx36x)
                (device as DmNvx36x).SetAudioDefaults(props);
        }

        public static void SetAudioDefaults(this DmNvx35x device, NvxDeviceProperties props)
        {
            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.EnableAutomaticInitiation();
                device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.SecondaryAudio.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
            }
            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
                device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();
            }
        }

        public static void SetAudioDefaults(this DmNvx36x device, NvxDeviceProperties props)
        {
            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.EnableAutomaticInitiation();

                device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.SecondaryAudio.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
            }
            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
                device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();
            }
        }
    }
}