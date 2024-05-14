using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class MulticastAddressFeedback
{
    public const string Key = "MulticastAddress";

    public static StringFeedback GetFeedback(DmNvxBaseClass device)
    {
        var feedback = new StringFeedback(Key,
            () => device.Control.MulticastAddressFeedback.StringValue);

        device.BaseEvent += (@base, args) => feedback.FireUpdate();
        if (device is not DmNvx35x nvx35X)
            return feedback;

        device.SourceReceive.StreamChange += (stream, args) => feedback.FireUpdate();
        device.SourceTransmit.StreamChange += (stream, args) => feedback.FireUpdate();

        return feedback;
    }
}