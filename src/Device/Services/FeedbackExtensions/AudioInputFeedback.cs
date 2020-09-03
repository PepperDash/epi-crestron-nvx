using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class AudioInputFeedback
    {
        public static readonly string NameKey = "AudioInput";
        public static readonly string ValueKey = "AudioInput";

        public static StringFeedback GetAudioInputFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NameKey, () => device.Control.ActiveAudioSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetAudioInputValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(ValueKey, () => (int)device.Control.ActiveAudioSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }

        /*public static void SetAudioRxInput(this DmNvxBaseClass device, AudioInputEnum input)
        {
            if (input == AudioInputEnum.AnalogAudio)
                return;

            device.SetAudioInput(input);
        }

        public static void SetAudioTxInput(this DmNvxBaseClass device, AudioInputEnum input)
        {
            if (input == AudioInputEnum.Stream)
                return;

            device.SetAudioInput(input);
        }

        public static DmNvxBaseClass SetAudioInput(this DmNvxBaseClass device, AudioInputEnum input)
        {
            device.Control.AudioSource = (DmNvxControl.eAudioSource)input.Value;
            return device;
        }*/
    }
}