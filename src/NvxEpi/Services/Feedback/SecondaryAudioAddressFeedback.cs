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

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            if (device.SecondaryAudio != null)
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            if (device.DmNaxRouting.DmNaxTransmit != null)
            {
                device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();
                device.DmNaxRouting.DmNaxRoutingChange += (sender, args) => feedback.FireUpdate();
            }
            else
            {
                throw new NotSupportedException("NAX/Secondary Audio");
            }

            return feedback;
        }

        public static StringFeedback GetFeedbackForReceiver(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key,
                () => device.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();
            }
            else if (device.DmNaxRouting.DmNaxTransmit != null)
            {
                device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();
                device.DmNaxRouting.DmNaxRoutingChange += (sender, args) => feedback.FireUpdate();
            }
            else
            {
                throw new NotSupportedException("NAX/Secondary Audio");
            }

            return feedback;
        }
    }
}