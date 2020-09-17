using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface IStream : INvxDevice
    {
        StringFeedback StreamUrl { get; }
        BoolFeedback IsStreamingVideo { get; }
        StringFeedback VideoStreamStatus { get; }
    }

    public static class NvxStreamExtensions
    {
        public static void StartStream(this IStream device)
        {
            if (device.Hardware.Control.StartFeedback.BoolValue)
                return;

            if (device.IsTransmiter)
            {
                device.Hardware.Control.EnableAutomaticInitiation();
                return;
            }

            Debug.Console(1, device, "Starting stream...");
            device.Hardware.Control.DisableAutomaticInitiation();
            device.Hardware.Control.Start();
        }

        public static void StopStream(this IStream device)
        {
            if (!device.Hardware.Control.StartFeedback.BoolValue)
                return;

            Debug.Console(1, device, "Stopping stream...");
            device.Hardware.Control.DisableAutomaticInitiation();
            device.Hardware.Control.Stop();
        }

        public static void SetVideoToInputStream(this IStream device)
        {
            if (device.IsTransmiter)
                return;

            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }

        public static bool TryGetCurrentStreamRoute(this IStream device, out IStream result)
        {
            result = device.GetCurrentStreamRoute();
            return result != null;
        }

        public static IStream GetCurrentStreamRoute(this IStream device)
        {
            try
            {
                if (device.IsTransmiter || device.DeviceId == default(int) ||
                    !device.IsStreamingVideo.BoolValue)
                    return null;

                if (device.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi1 ||
                    device.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi2 ||
                    device.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return null;

                var result = DeviceManager
                    .AllDevices
                    .OfType<IStream>()
                    .Where(t => t.IsTransmiter && t.IsStreamingVideo.BoolValue)
                    .FirstOrDefault(
                        x =>
                            x.MulticastAddress.StringValue.Equals(device.MulticastAddress.StringValue,
                                StringComparison.OrdinalIgnoreCase));

                return result;
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error getting current video route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }
}