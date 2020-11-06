using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class SecondaryAudioAddressFeedback
    {
        public const string Key = "SecondaryAudioAddress";

        public static StringFeedback GetFeedback(DmNvx35x device)
        {
            if (device.SecondaryAudio == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(Key,
                () => device.SecondaryAudio.MulticastAddressFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static StringFeedback GetFeedback(DmNvxE3x device)
        {
            var feedback = new StringFeedback(Key,
                () => String.Empty);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}