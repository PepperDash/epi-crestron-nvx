using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using NvxEpi.Abstractions;

namespace NvxEpi.Services.Feedback
{
    public class AudioRxAddressFeedback
    {
        public const string Key = "RxAudioAddress";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            if (device.SecondaryAudio != null)
            {
                device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();
            }
            else if (device.DmNaxRouting.DmNaxReceive != null)
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