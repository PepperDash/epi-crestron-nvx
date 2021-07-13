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
            var feedback= new StringFeedback(Key,
                    () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback.ToString());
            
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
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback.ToString());
            
            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            if (device.SecondaryAudio != null)
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            if (device.DmNaxRouting.DmNaxReceive != null)
            {
                device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();
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