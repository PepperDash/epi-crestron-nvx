using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.Feedback
{
    public class Hdmi1SyncDetectedFeedback
    {
        public const string Key = "Hdmi1SyncDetected";

        private static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var feedback = new BoolFeedback(Key, 
                () => device.HdmiIn[1].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }

        public static BoolFeedback GetFeedback(DmNvx35x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }

        public static BoolFeedback GetFeedback(DmNvxE3x device)
        {
            return GetFeedback(device as DmNvxBaseClass);
        }
    }
}