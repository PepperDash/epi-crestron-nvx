using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingNaxRxAudioFeedback
    {
        public static readonly string Key = "IsStreamingNaxRxAudio";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, 
                () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback == DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);

            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}