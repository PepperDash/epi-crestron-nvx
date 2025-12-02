using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

public class Usbc2HdcpCapabilityValueFeedback
{
    public const string Key = "Usbc2HdcpCapabilityValue";

    public static IntFeedback GetFeedback(DmNvx38x device)
    {
        if (device.UsbcIn == null || device.UsbcIn[2] == null)
            return new IntFeedback(() => 0);

        var feedback = new IntFeedback(Key,
            () => (int)device.UsbcIn[2].HdcpCapabilityFeedback);

        device.UsbcIn[2].StreamChange += (stream, args) => feedback.FireUpdate();
        return feedback;
    }
}