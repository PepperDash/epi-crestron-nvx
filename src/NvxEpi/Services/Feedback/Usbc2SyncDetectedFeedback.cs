using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class Usbc2SyncDetectedFeedback
{
    public const string Key = "Usbc2SyncDetected";

    public static BoolFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[2] == null)
            return new BoolFeedback(() => false);

        var feedback = new BoolFeedback(Key, 
            () => device.UsbcIn[2].SyncDetectedFeedback.BoolValue);

        device.UsbcIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}