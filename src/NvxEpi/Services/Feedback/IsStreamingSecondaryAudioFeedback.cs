using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class IsStreamingSecondaryAudioFeedback
    {
        public static readonly string Key = "IsStreamingSecondaryAudio";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key,
                () =>
                    {
                        if (device.Control.ActiveAudioSourceFeedback != DmNvxControl.eAudioSource.SecondaryStreamAudio)
                            return false;

                        return device.Control.ActiveVideoSourceFeedback != eSfpVideoSourceTypes.Disable &&
                               device.SecondaryAudio.StartFeedback.BoolValue;
                    });

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            device.SecondaryAudio.SecondaryAudioChange += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}