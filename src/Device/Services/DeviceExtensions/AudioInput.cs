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

        public static void SetAudioRxInput(this DmNvxBaseClass device, AudioInputEnum input)
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
        }
    }
}