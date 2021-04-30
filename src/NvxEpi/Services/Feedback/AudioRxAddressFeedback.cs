using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using NvxEpi.Abstractions;

namespace NvxEpi.Services.Feedback
{
    public class AudioRxAddressFeedback
    {
        public const string Key = "RxAudioAddress";

        public static StringFeedback GetFeedback(INvxDeviceWithHardware device)
        {
            var hardware = device.Hardware;
            var feedback = new StringFeedback(Key,
                () => GetSecondaryAudioFeedbackHelper(device));

            hardware.BaseEvent += (@base, args) => feedback.FireUpdate();

            if (hardware.SecondaryAudio != null)
            {
                hardware.SecondaryAudio.SecondaryAudioChange += (sender, args) => feedback.FireUpdate();
            }
            else if (hardware.DmNaxRouting.DmNaxReceive != null)
            {
                hardware.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += (sender, args) => feedback.FireUpdate();
                hardware.DmNaxRouting.DmNaxRoutingChange += (sender, args) => feedback.FireUpdate();
            }
            else
            {
                throw new NotSupportedException("NAX/Secondary Audio");
            }

            return feedback;
        }

        public static string GetSecondaryAudioFeedbackHelper(INvxDeviceWithHardware device)
        {
            if (device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.DmNaxAudio)
            {
                return device.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue;
            }
            else
            {
                return device.AudioSourceName.StringValue;
            }
        }
    }
}