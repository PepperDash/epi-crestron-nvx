using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public static class CurrentStreamRouteExtensions
    {
        public static bool TryGetCurrentStreamRoute(this IStream streamm, out IStream result)
        {
            result = streamm.GetCurrentStreamRoute();
            return result != null;
        }

        public static IStream GetCurrentStreamRoute(this IStream stream)
        {
            try
            {
                if (CheckIfDeviceIsNotStreaming(stream))
                    return null;

                var result = DeviceManager
                    .AllDevices
                    .OfType<IStream>()
                    .Where(t => t.IsTransmitter && t.IsStreamingVideo.BoolValue)
                    .FirstOrDefault(
                        x =>
                            x.StreamUrl.StringValue.Equals(stream.StreamUrl.StringValue,
                                StringComparison.OrdinalIgnoreCase));

                return result;
            }
            catch (Exception ex)
            {
                Debug.Console(1, stream, "Error getting current video route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }

        private static bool CheckIfDeviceIsNotStreaming(IStream stream)
        {
            if (stream.IsTransmitter || stream.DeviceId == default(int) ||
                !stream.IsStreamingVideo.BoolValue || !stream.IsOnline.BoolValue)
            {
                Debug.Console(1, stream, "Device not streaming...");
                return true;
            }

            if (stream.Hardware.Control.VideoSourceFeedback != eSfpVideoSourceTypes.Hdmi1 &&
                stream.Hardware.Control.VideoSourceFeedback != eSfpVideoSourceTypes.Hdmi2 &&
                stream.Hardware.Control.VideoSourceFeedback != eSfpVideoSourceTypes.Disable) 
                return false;

            Debug.Console(1, stream, "Device not on Stream Input... {0}",
                stream.Hardware.Control.VideoSourceFeedback.ToString());

            return true;
        }
    }
}