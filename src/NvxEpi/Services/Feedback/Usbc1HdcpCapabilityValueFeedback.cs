using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class Usbc1HdcpCapabilityValueFeedback
{
    public const string Key = "Usbc1HdcpCapabilityValue";

    public static IntFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[1] == null)
            return new IntFeedback(() => 0);

        var feedback = new IntFeedback(Key,
            () => (int)device.UsbcIn[1].HdcpCapabilityFeedback);

        device.UsbcIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}