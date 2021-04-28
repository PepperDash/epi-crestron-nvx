using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingSecondaryAudioFeedback
    {
        public static readonly string Key = "IsStreamingSecondaryAudio";

        public static BoolFeedback GetFeedbackForTransmitter(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () =>
                    device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback == DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);
            
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

        public static BoolFeedback GetFeedbackForReceiver(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () =>
                    device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback == DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);
            
            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            if (device.SecondaryAudio != null)
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();

            if (device.DmNaxRouting.DmNaxTransmit != null)
            {
                device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();
                device.DmNaxRouting.DmNaxRoutingChange += (@base, args) => feedback.FireUpdate();
            }
            else
            {
                throw new NotSupportedException("NAX/Secondary Audio");
            }

            return feedback;
        }
    }
}