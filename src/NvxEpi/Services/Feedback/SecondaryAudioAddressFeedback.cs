using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class SecondaryAudioAddressFeedback
    {
        public const string Key = "SecondaryAudioAddress";

        public static StringFeedback GetFeedbackForTransmitter(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxTransmit.MulticastAddressFeedback.StringValue);

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static StringFeedback GetFeedbackForReceiver(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue);

            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}