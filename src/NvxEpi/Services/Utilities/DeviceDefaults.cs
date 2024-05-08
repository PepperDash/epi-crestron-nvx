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
            if (!(device is DmNvxE3x))
            {
                device.Control.DeviceMode = eDeviceMode.Transmitter;
            }

            device.Control.EnableAutomaticInitiation();
            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
            {
                Debug.Console(0, "Setting multicast Video Address to {0}", props.MulticastVideoAddress);
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;
            }

            SetDefaultInputsFromConfig(device, props);
            device.SetTxAudioDefaults(props);
        }

        private static void SetDefaultInputsFromConfig(DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (!string.IsNullOrEmpty(props.DefaultVideoInput))
            {
                try
                {
                    device.Control.VideoSource =
                        (eSfpVideoSourceTypes) Enum.Parse(typeof (eSfpVideoSourceTypes), props.DefaultVideoInput, true);
                }
                catch (Exception ex)
                {
                    Debug.Console(1, "Cannot set device to video input:{0} | {1}", props.DefaultVideoInput, ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(props.DefaultAudioInput))
            {
                try
                {
                    device.Control.AudioSource =
                        (DmNvxControl.eAudioSource)
                            Enum.Parse(typeof (DmNvxControl.eAudioSource), props.DefaultAudioInput, true);
                }
                catch (Exception ex)
                {
                    Debug.Console(1, "Cannot set device to audio input:{0} | {1}", props.DefaultVideoInput, ex.Message);
                }
            }
        }

        public static void SetRxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (!(device is DmNvxD3x))
            {
                device.Control.DeviceMode = eDeviceMode.Receiver;
            }

            device.Control.EnableAutomaticInitiation();
            SetDefaultInputsFromConfig(device, props);
            device.SetRxAudioDefaults(props);
        }

        public static void SetTxAudioDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
                device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();

                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    //For a TX, multicast audio address is optional, if it isn't defined it will auto generate from the video multicast address +1
                    device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.DmNaxRouting.DmNaxTransmit.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
                else
                {
                    device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                }
            }
            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.EnableAutomaticInitiation();
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    //For a TX, multicast audio address is optional, if it isn't defined it will auto generate from the video multicast address +1
                    device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                    device.SecondaryAudio.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
                else
                {
                    device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
                }
            }
        }

        public static void SetRxAudioDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            if (device.DmNaxRouting != null)
            {
                device.DmNaxRouting.DmNaxReceive.EnableAutomaticInitiation();
                device.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();

                //Receivers should be in manual mode all the time - auto mode is only if you want NaxAudio to follow the video route, which is not common
                device.DmNaxRouting.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    //This is only true if the rx is being used to transmit audio, in which case config MUST define the multicast address
                    //There is no video multicast address on a receiver so no ability to deduct multicast audio address from the video address
                    device.DmNaxRouting.DmNaxTransmit.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
            }
            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.EnableAutomaticInitiation();

                //Receivers should be in manual mode all the time - auto mode is only if you want NaxAudio to follow the video route, which is not common
                device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
                {
                    //This is only true if the rx is being used to transmit audio, in which case config MUST define the multicast address
                    //There is no video multicast address on a receiver so no ability to deduct multicast audio address from the video address
                    device.SecondaryAudio.MulticastAddress.StringValue = props.MulticastAudioAddress;
                }
            }
        }
    }
}