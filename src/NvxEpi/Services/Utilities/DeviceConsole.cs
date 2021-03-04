using System;
using System.Linq;
using NvxEpi.Abstractions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Utilities
{
    public class DeviceConsole
    {
        public static void PrintInfoForAllDevices()
        {
            var devices = DeviceManager
                .GetDevices()
                .OfType<INvxDevice>();

            foreach (var device in devices)
            {
                Debug.Console(1, device, "----------- {0} -----------", device.Name);
                PrintInfoToConsole(device);
                Debug.Console(1, device, "-----------------------------------------\r");
            }       
        }

        public static void PrintInfoToConsole(IHasFeedback device)
        {
            foreach (var feedback in device.Feedbacks.Where(x => !String.IsNullOrEmpty(x.Key)))
            {
                if (feedback is BoolFeedback)
                    Debug.Console(1, device, "{0} : '{1}'", feedback.Key, feedback.BoolValue);

                if (feedback is IntFeedback)
                    Debug.Console(1, device, "{0} : '{1}'", feedback.Key, feedback.IntValue);

                if (feedback is StringFeedback)
                    Debug.Console(1, device, "{0} : '{1}'", feedback.Key, feedback.StringValue);
            }
        }
    }
}