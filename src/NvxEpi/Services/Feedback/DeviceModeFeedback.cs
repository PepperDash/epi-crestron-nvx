using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DeviceModeFeedback
    {
        public const string Key = "DeviceModeValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
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