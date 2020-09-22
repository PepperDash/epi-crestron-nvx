using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Entities.Config;
using PepperDash.Core;

namespace NvxEpi.Device.Services.Utilities
{
    //TODO : Add NAX audio to this
    public static class DeviceDefaults
    {
        public static void SetTxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Transmitter;
            device.Control.EnableAutomaticInitiation();
            device.SecondaryAudio.EnableAutomaticInitiation();

            if (!String.IsNullOrEmpty(props.MulticastVideoAddress))
                device.Control.MulticastAddress.StringValue = props.MulticastVideoAddress;

            if (!String.IsNullOrEmpty(props.MulticastAudioAddress))
            {
                device.SecondaryAudio.SecondaryAudioMode =
                    DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                device.SecondaryAudio.MulticastAddress.StringValue = props.MulticastAudioAddress;
            }
            else
            {
                device.SecondaryAudio.SecondaryAudioMode =
                    DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
            }
        }

        public static void SetRxDefaults(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Receiver;
            device.Control.DisableAutomaticInitiation();
            device.SecondaryAudio.DisableAutomaticInitiation();
            device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
        }

        public static void SetDefaultInputs(this DmNvxBaseClass device, NvxDeviceProperties props)
        {
            try
            {
                if (!String.IsNullOrEmpty(props.DefaultVideoSource))
                {
                    var videoInput =
                        (eSfpVideoSourceTypes)Enum.Parse(typeof(eSfpVideoSourceTypes), props.DefaultVideoSource, true);

                    device.Control.VideoSource = videoInput;
                }

                if (String.IsNullOrEmpty(props.DefaultAudioSource))
                    return;

                var audioInput =
                    (DmNvxControl.eAudioSource)
                        Enum.Parse(typeof(DmNvxControl.eAudioSource), props.DefaultAudioSource, true);

                device.Control.AudioSource = audioInput;
            }
            catch (ArgumentException ex)
            {
                Debug.Console(1, "Cannot set default input, argument not resolved:{0}", ex.Message);
            }
        }
    }
}