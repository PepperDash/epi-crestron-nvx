using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class MultiviewLayoutFeedback
{
    public const string Key = "MultiviewLayout";

    public static IntFeedback GetFeedback(DmNvxBaseClass device)
    {
        if (device.HdmiOut == null)
            return new IntFeedback(Key, () => 0);

        // TODO - Identify correct feedback, requires Crestron SimplSharpPro reference update in Essentials
        var feedback = new IntFeedback(Key, () => 0);
        // var feedback = new IntFeedback(Key, () => device.HdmiOut.MultiviewLayoutFeedback.UShortValue);
        device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

        return feedback;
    }

}
