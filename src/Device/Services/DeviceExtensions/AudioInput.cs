using System.Collections.Generic;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class AudioInput
    {
        public static StringFeedback GetAudioInputFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.AudioInputName.ToString(),
                () => device.Control.ActiveAudioSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }

        public static IntFeedback GetAudioInputValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(NvxDevice.DeviceFeedbacks.AudioInputValue.ToString(),
                () => (int)device.Control.ActiveAudioSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }

        /*public static void SetAudioInput(this DmNvxBaseClass device, string input)
        {
            AudioInputEnum result;
            if (!AudioInputEnum.TryFromNameNoSpaces(input, out result))
                return;

            device.SetAudioInput(result);
        }

        public static void SetAudioInput(this DmNvxBaseClass device, int input)
        {
            AudioInputEnum result;
            if (!AudioInputEnum.TryFromValue(input, out result))
                return;

            device.SetAudioInput(result);
        }

        public static DmNvxBaseClass SetAudioInput(this DmNvxBaseClass device, AudioInputEnum input)
        {
            device.Control.AudioSource = (DmNvxControl.eAudioSource)input.Value;
            return device;
        }*/
    }
}