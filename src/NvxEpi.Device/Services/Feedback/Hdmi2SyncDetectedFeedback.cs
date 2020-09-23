using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.Feedback
{
    public class Hdmi2SyncDetectedFeedback
    {
        public const string Key = "Hdmi2SyncDetected";

        public static BoolFeedback GetFeedback(DmNvx35x device)
        {
            var feedback = new BoolFeedback(Key,
                () => device.HdmiIn[2].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}