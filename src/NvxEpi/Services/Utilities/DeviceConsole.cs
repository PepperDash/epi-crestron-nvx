using System.Linq;
using NvxEpi.Abstractions;
using PepperDash.Core.Logging;
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
            device.LogInformation("----------- {0} -----------", device.Name);
            PrintInfoToConsole(device);
            device.LogInformation("-----------------------------------------\r");
        }
    }

    private static void PrintInfoToConsole(IHasFeedback device)
    {
        foreach (var feedback in device.Feedbacks.Where(x => x != null && !string.IsNullOrEmpty(x.Key)))
        {
            if (feedback is BoolFeedback)
                device.LogInformation("{0} : '{1}'", feedback.Key, feedback.BoolValue);

            if (feedback is IntFeedback)
                device.LogInformation("{0} : '{1}'", feedback.Key, feedback.IntValue);

            if (feedback is StringFeedback)
                device.LogInformation("{0} : '{1}'", feedback.Key, feedback.StringValue);
        }
    }
}