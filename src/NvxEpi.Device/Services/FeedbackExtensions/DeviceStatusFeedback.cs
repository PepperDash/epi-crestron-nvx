using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class DeviceStatusFeedback
    {
        public static readonly string Key = DeviceFeedbackEnum.DeviceStatus.ToString();

        public static StringFeedback GetDeviceStatusFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(Key, () => device.Control.StatusTextFeedback.StringValue);

            device.BaseEvent += (@base, args) =>
            {
                if (args.EventId != DMInputEventIds.StatusTextEventId)
                    return;

                feedback.FireUpdate();
            };

            return feedback;
        }      
    }
}