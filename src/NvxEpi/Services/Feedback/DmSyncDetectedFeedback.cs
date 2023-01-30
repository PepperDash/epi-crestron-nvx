using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DmSyncDetectedFeedback
    {
        public const string Key = "Hdmi1SyncDetected";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device)
        {
            var dmDevice = device as DmNvxE760x;
            if (dmDevice == null)
                return new BoolFeedback(() => false);

            var feedback = new BoolFeedback(Key,
                () => device.DmIn.SyncDetectedFeedback.BoolValue);

            device.DmIn.InputStreamChange += (stream, args) => feedback.FireUpdate();
            device.DmIn.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
}