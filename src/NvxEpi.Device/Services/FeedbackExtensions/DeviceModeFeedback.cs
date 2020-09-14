using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class DeviceModeFeedback
    {
        public static readonly string Key = DeviceFeedbackEnum.DeviceMode.ToString();

        public static IntFeedback GetDeviceModeFeedback(this DmNvxBaseClass device)
        {
            if (device is DmNvxD3x)
                return new IntFeedback(Key, () => (int) eDeviceMode.Receiver);

            if (device is DmNvxE3x)
                return new IntFeedback(Key, () => (int) eDeviceMode.Transmitter);

            var feedback = new IntFeedback(Key, () => (int) device.Control.DeviceModeFeedback);
            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}