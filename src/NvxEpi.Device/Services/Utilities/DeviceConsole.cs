using System.Linq;
using NvxEpi.Abstractions.Device;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.Utilities
{
    public class DeviceConsole
    {
        public static void PrintInfoForAllDevices()
        {
            var devices = DeviceManager.GetDevices()
                .OfType<INvxDevice>();

            foreach (var device in devices)
                PrintInfoToConsole(device);
        }

        public static void PrintInfoToConsole(INvxDevice device)
        {
            foreach (var feedback in device.Feedbacks)
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