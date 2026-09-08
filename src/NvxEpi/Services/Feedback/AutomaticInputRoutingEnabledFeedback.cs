using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class AutomaticInputRoutingEnabledFeedback
{
    public const string Key = "AutomaticInputRoutingEnabled";

    public static BoolFeedback GetFeedback(DmNvxBaseClass device)
    {
        var feedback = new BoolFeedback(Key, () =>
        {
            // Some NVX models/firmware don't populate this join - Crestron returns its
            // internal NullSig sentinel (not a C# null) for it, so BoolValue throws rather
            // than the reference being null. A null-conditional access can't catch that.
            try
            {
                return device.Control.EnableAutomaticInputRoutingFeedback.BoolValue;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        });

        device.BaseEvent += (@base, args) => feedback.FireUpdate();
        return feedback;
    }
}
