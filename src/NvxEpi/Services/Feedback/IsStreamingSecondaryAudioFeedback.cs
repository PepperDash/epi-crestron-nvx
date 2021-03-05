using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingSecondaryAudioFeedback
    {
        public static readonly string Key = "IsStreamingSecondaryAudio";

        public static BoolFeedback GetFeedbackForTransmitter(DmNvxBaseClass device)
        {
            // TODO - investigate if this actually works with a source connected
            var feedback = new BoolFeedback(Key,
                () =>
                    device.SecondaryAudio.StartFeedback.BoolValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();
            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();
            device.DmNaxRouting.DmNaxRoutingChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static BoolFeedback GetFeedbackForReceiver(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () =>
                    device.SecondaryAudio.StartFeedback.BoolValue);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();
            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();
            device.DmNaxRouting.DmNaxRoutingChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}