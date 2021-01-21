using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class StreamExtensions
    {
        private static readonly string _noRouteAddress = String.Empty;

        public static void ClearStream(this IStream device)
        {
            if (device.IsTransmitter)
                return;

            Debug.Console(1, device, "Clearing stream");
            device.Hardware.Control.ServerUrl.StringValue = _noRouteAddress;
        }

        public static void RouteStream(this IStream device, IStream tx)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (tx == null)
            {
                device.ClearStream();
                return;
            }

            if (!tx.IsTransmitter)
                throw new ArgumentException("tx");

            Debug.Console(1, device, "Routing device stream : '{0}'", tx.Name);
            tx.MulticastAddress.FireUpdate();
            tx.StreamUrl.FireUpdate();
            device.SetStreamUrl(tx.StreamUrl.StringValue);
        }

        public static void SetStreamUrl(this IStream device, string url)
        {
            if (device.IsTransmitter)
                return;

            if (String.IsNullOrEmpty(url))
                return;

            Debug.Console(1, device, "Setting stream: '{0}", url);
            device.Hardware.Control.ServerUrl.StringValue = url;
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }
    }
}