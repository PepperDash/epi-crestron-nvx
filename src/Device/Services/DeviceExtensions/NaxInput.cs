using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class NaxInput
    {
        public static StringFeedback GetNaxInputFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NvxDevice.DeviceFeedbacks.NaxInput.ToString(),
                () => device.Control.ActiveDmNaxAudioSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static void SetNaxInput(this DmNvxBaseClass device, string input)
        {
            if (!ValidateDevice(device))
                return;

            NaxInputEnum result;
            if (!NaxInputEnum.TryFromNameNoSpaces(input, out result))
                return;

            device.Control.VideoSource = (eSfpVideoSourceTypes)result.Value;
        }

        public static void SetNaxInput(this DmNvxBaseClass device, int input)
        {
            if (!ValidateDevice(device))
                return;

            NaxInputEnum result;
            if (!NaxInputEnum.TryFromValue(input, out result))
                return;

            device.Control.VideoSource = (eSfpVideoSourceTypes)result.Value;
        }

        public static DmNvxBaseClass SetNaxInput(this DmNvxBaseClass device, NaxInputEnum input)
        {
            if (!ValidateDevice(device))
                return device;

            device.Control.VideoSource = (eSfpVideoSourceTypes)input.Value;
            return device;
        }

        private static bool ValidateDevice(DmNvxBaseClass device)
        {
            return device is DmNvx35x;
        }
    }
}