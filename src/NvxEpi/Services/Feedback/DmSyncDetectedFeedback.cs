using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class DmSyncDetectedFeedback
{
    public const string Key = "DmInSyncDetected";

    public static BoolFeedback GetFeedback(DmNvxBaseClass device)
    {
        if (device is not DmNvxE760x dmDevice)
            return new BoolFeedback(Key, () => false);

        var feedback = new BoolFeedback(Key,
            () => device.DmIn.SyncDetectedFeedback.BoolValue);

        device.DmIn.InputStreamChange += (stream, args) => feedback.FireUpdate();
        device.DmIn.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}