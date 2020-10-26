using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingNaxTxAudioFeedback
    {
        public static readonly string Key = "IsStreamingNaxTxAudio";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback == DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);

            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}