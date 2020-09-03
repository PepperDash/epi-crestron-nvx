using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class DeviceNameFeedback
    {
        public static readonly string Key = "DeviceName";

        public static StringFeedback GetDeviceNameFeedback(this DmNvxBaseClass device, DeviceConfig config)
        {
            var name = config.Key;
            device.Control.Name.StringValue = name.Replace(" ", "-");

            var feedback = new StringFeedback(Key, () => config.Name);
            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}