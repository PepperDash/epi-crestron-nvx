using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class DmHdcpCapabilityValueFeedback
    {
        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            var dmDevice = device as DmNvxE760x;
            if (dmDevice == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(Hdmi1HdcpCapabilityValueFeedback.Key,
                () => (int)device.DmIn.HdcpCapabilityFeedback);

            device.DmIn.InputStreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

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
            return feedback;
        }
    }
}