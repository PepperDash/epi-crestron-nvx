using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class SecondaryAudio
    {
        public static StringFeedback GetSecondaryAudioStatusFeedback(this DmNvxBaseClass device)
        {
            if (device.SecondaryAudio == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.SecondaryAudioStatus.ToString(), 
                () => device.SecondaryAudio.StatusFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static StringFeedback GetSecondaryAudioAddressFeedback(this DmNvxBaseClass device)
        {
            if (device.SecondaryAudio == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.SecondaryAudioAddress.ToString(),
                () => device.SecondaryAudio.MulticastAddressFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        /*public static DmNvxBaseClass SetSecondaryAudioMulticastAddress(this DmNvxBaseClass device, string address)
        {
            if (device.SecondaryAudio == null)
                return device;

            if (String.IsNullOrEmpty(address))
                return device;

            device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
            device.SecondaryAudio.MulticastAddress.StringValue = address;
            return device;
        }*/
    }
}