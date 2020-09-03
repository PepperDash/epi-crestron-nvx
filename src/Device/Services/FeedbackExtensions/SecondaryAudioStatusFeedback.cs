using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models.Entities;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class SecondaryAudioStatusFeedback
    {
        public static readonly string Key = "SecondaryAudioStatus";
        public static StringFeedback GetSecondaryAudioStatusFeedback(this DmNvxBaseClass device)
        {
            if (device.SecondaryAudio == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(Key, 
                () => device.SecondaryAudio.StatusFeedback.ToString());

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