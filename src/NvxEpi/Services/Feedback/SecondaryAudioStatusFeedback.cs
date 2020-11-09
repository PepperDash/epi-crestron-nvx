using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class SecondaryAudioStatusFeedback
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

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var dmNvx35x = device as DmNvx35x;
            if (dmNvx35x != null)
                return GetFeedback(dmNvx35x);

            throw new NotSupportedException(device.GetType().Name);
        }
    }
}