using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceFeedback
{
    public class SecondaryAudioStatus
    {
        public const string Key = "SecondaryAudioStatus";

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            if (device.SecondaryAudio == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(Key, 
                () => device.SecondaryAudio.StatusFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}