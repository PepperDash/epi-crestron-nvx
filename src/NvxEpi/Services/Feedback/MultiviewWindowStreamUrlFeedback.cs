using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class MultiviewWindowStreamUrlFeedback
{
    public const string Key = "MultiviewWindowStreamUrl";

    public static StringFeedback GetFeedback(DmNvxBaseClass device, char window)
    {
        if (device.HdmiOut == null)
            return new StringFeedback(Key, () => "");

        // TODO - Identify correct feedback, requires Crestron SimplSharpPro reference update in Essentials
        var feedback = new StringFeedback(Key, () => "");
        // var feedback = new StringFeedback(Key, () => device.HdmiOut.MultiviewWindowStreamUrl.StringValue);
        device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();

        return feedback;
    }

}
