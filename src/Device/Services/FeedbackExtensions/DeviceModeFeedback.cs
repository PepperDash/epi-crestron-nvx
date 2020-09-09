using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class DeviceModeFeedback
    {
        public static readonly string Key = "DeviceMode";

        public static IntFeedback GetDeviceModeFeedback(this DmNvxBaseClass device)
        {
            if (device is DmNvxD3x)
                return new IntFeedback(Key, () => (int) DeviceModeEnum.Receiver);

            if (device is DmNvxE3x)
                return new IntFeedback(Key, () => (int) DeviceModeEnum.Transmitter);

            var feedback = new IntFeedback(Key, () => (int) device.Control.DeviceModeFeedback);
            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        /*public static DmNvxBaseClass SetDeviceMode(this DmNvxBaseClass device, DeviceModeEnum mode)
        {
            return mode == DeviceModeEnum.Transmitter ? device.IsTransmitter() : device.IsReceiver();
        }

        public static DmNvxBaseClass IsTransmitter(this DmNvxBaseClass device)
        {
            if (device is DmNvxD3x)
                return device;

            device.Control.DeviceMode = eDeviceMode.Transmitter;
            return device;
        }

        public static DmNvxBaseClass IsReceiver(this DmNvxBaseClass device)
        {
            if (device is DmNvxE3x)
                return device;

            device.Control.DeviceMode = eDeviceMode.Receiver;
            return device;
        }*/
    }
}