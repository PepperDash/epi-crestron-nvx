using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class Usbc1SyncDetectedFeedback
{
    public const string Key = "Usbc1SyncDetected";

    public static BoolFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[1] == null)
            return new BoolFeedback(() => false);

        var feedback = new BoolFeedback(Key, 
            () => device.UsbcIn[1].SyncDetectedFeedback.BoolValue);

        device.UsbcIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}