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
                if (!stream.IsStreamingVideo.BoolValue)
                    return null;

                if (stream.Hardware.Control.ActiveVideoSourceFeedback != eSfpVideoSourceTypes.Stream)
                    return null;

                return DeviceManager
                    .AllDevices
                    .OfType<IStream>()
                    .Where(t => t.IsTransmitter)
                    .FirstOrDefault(tx => tx.StreamUrl.StringValue.Equals(stream.StreamUrl.StringValue, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Debug.Console(1, stream, "Error getting current video route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }
}