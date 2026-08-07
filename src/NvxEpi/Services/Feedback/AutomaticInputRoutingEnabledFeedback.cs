using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class AutomaticInputRoutingEnabledFeedback
{
    public const string Key = "AutomaticInputRoutingEnabled";

    public static BoolFeedback GetFeedback(DmNvxBaseClass device)
    {
        var feedback = new BoolFeedback(Key,
            () => device.Control.EnableAutomaticInputRoutingFeedback.BoolValue);

        device.BaseEvent += (@base, args) => feedback.FireUpdate();
        return feedback;
    }
}
