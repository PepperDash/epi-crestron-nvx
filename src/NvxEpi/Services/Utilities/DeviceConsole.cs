using System;
using System.Linq;
using NvxEpi.Abstractions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Utilities;

public static class DeviceConsole
{
    public static void PrintInfoForAllDevices()
    {
        var devices = DeviceManager
            .GetDevices()
            .OfType<INvxDevice>();

        foreach (var device in devices)
        {
            Debug.LogMessage(0, device, "----------- {0} -----------", device.Name);
            PrintInfoToConsole(device);
            Debug.LogMessage(0, device, "-----------------------------------------\r");
        }       
    }

    private static void PrintInfoToConsole(IHasFeedback device)
    {
        foreach (var feedback in device.Feedbacks.Where(x => x != null && !string.IsNullOrEmpty(x.Key)))
        {
            if (feedback is BoolFeedback)
                Debug.LogMessage(0, device, "{0} : '{1}'", feedback.Key, feedback.BoolValue);

            if (feedback is IntFeedback)
                Debug.LogMessage(0, device, "{0} : '{1}'", feedback.Key, feedback.IntValue);

            if (feedback is StringFeedback)
                Debug.LogMessage(0, device, "{0} : '{1}'", feedback.Key, feedback.StringValue);
        }
    }
}