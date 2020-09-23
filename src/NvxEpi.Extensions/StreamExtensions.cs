using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class StreamExtensions
    {
        public static void StartStream(this IStream device)
        {
            if (device.Hardware.Control.StartFeedback.BoolValue)
                return;

            if (device.IsTransmitter)
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

        public static void SetStreamUrl(this IStream device, string url)
        {
            if (String.IsNullOrEmpty(url))
                return;

            Debug.Console(1, device, "Setting stream: '{0}", url);
            device.Hardware.Control.ServerUrl.StringValue = url;
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
            device.StartStream();
        }

        public static void RouteStream(this IStream device, ushort txId)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (txId == 0)
            {
                device.StopStream();
                return;
            }

            var tx = StreamUtilities.GetTxById(txId);
            if (tx == null)
                return;

            device.RouteStream(tx);
        }

        public static void RouteStream(this IStream device, IStream tx)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (tx == null)
            {
                device.StopStream();
                return;
            }

            if (!tx.IsTransmitter)
                throw new ArgumentException("tx");

            Debug.Console(1, device, "Routing device stream : '{0}'", tx.Name);
            device.SetStreamUrl(tx.StreamUrl.StringValue);
        }
    }
}