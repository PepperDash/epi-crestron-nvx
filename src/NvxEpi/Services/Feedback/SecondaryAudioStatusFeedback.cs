using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class SecondaryAudioStatusFeedback
    {
        public const string Key = "SecondaryAudioStatus";

        public static StringFeedback GetFeedbackForTransmitter(DmNvxBaseClass device)
        {
            if (device.DmNaxRouting.DmNaxTransmit == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }

        public static StringFeedback GetFeedbackForReceiver(DmNvxBaseClass device)
        {
            if (device.DmNaxRouting.DmNaxReceive == null)
                throw new NotSupportedException("Secondary Audio");

            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}