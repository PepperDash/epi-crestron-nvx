﻿using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Stream
{
    public interface ICurrentStream : IStream
    {
        StringFeedback CurrentStreamName { get; }
        IntFeedback CurrentStreamId { get; }
    }

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
                if (stream.IsTransmitter || stream.DeviceId == default(int) ||
                    !stream.IsStreamingVideo.BoolValue || !stream.Hardware.IsOnline)
                    return null;

                if (stream.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi1 ||
                    stream.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi2 ||
                    stream.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return null;

                var result = DeviceManager
                    .AllDevices
                    .OfType<IStream>()
                    .Where(t => t.IsTransmitter && t.IsStreamingVideo.BoolValue)
                    .FirstOrDefault(
                        x =>
                            x.MulticastAddress.StringValue.Equals(stream.MulticastAddress.StringValue,
                                StringComparison.OrdinalIgnoreCase));

                return result;
            }
            catch (Exception ex)
            {
                Debug.Console(1, stream, "Error getting current video route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }
}