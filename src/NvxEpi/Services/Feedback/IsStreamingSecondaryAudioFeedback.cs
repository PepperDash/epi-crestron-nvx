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
                    device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback ==
                    DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static BoolFeedback GetFeedbackForReceiver(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () =>
                    device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback ==
                    DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}