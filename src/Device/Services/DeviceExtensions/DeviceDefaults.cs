using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.Config;
using PepperDash.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    //TODO : Add NAX audio to this
    public static class DeviceDefaults
    {
        public static void SetDeviceDefaults(this INvxDevice device)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(device.Config);
            if (device.IsTransmitter.BoolValue)
                SetTxDefaults(device.Hardware, props);
            else
                SetRxDefaults(device.Hardware, props);

            SetDefaultInputs(device.Hardware, props);
        }

        private static void SetTxDefaults(DmNvxBaseClass device, NvxDeviceProperties props)
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

        private static void SetRxDefaults(DmNvxBaseClass device, NvxDeviceProperties props)
        {
            device.Control.DeviceMode = eDeviceMode.Receiver;
            device.Control.DisableAutomaticInitiation();
            device.SecondaryAudio.DisableAutomaticInitiation();
            device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
        }

        private static void SetDefaultInputs(DmNvxBaseClass device, NvxDeviceProperties props)
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