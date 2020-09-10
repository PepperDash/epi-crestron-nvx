using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class InputSyncDetectedFeedback
    {
        public static string Hdmi1Key = DeviceFeedbackEnum.Hdmi1SyncDetected.ToString();
        public static string Hdmi2Key = DeviceFeedbackEnum.Hdmi2SyncDetected.ToString();

        public static BoolFeedback GetHdmiIn1SyncDetectedFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Hdmi1Key, 
                () => device.HdmiIn[1].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static BoolFeedback GetHdmiIn2SyncDetectedFeedback(this DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Hdmi2Key, 
                () => device.HdmiIn[2].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}