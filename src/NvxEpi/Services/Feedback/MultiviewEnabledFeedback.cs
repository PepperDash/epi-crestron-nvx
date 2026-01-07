using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class MultiviewEnabledFeedback
{
    public const string Key = "MultiviewEnabled";

    public static BoolFeedback GetFeedback(DmNvxBaseClass device)
    {
        if (device.HdmiOut == null)
            return new BoolFeedback(Key, () => false);

        // TODO - Identify correct feedback, requires Crestron SimplSharpPro reference update in Essentials
        var feedback = new BoolFeedback(Key, () => false);
        // var feedback = new BoolFeedback(Key, () => device.HdmiOut.MultiviewEnabled.BoolValue);
        device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

        return feedback;
    }

}
