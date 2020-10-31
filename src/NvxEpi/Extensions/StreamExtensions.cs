using System;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class StreamExtensions
    {
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

            tx.StreamUrl.FireUpdate();
            device.SetStreamUrl(tx.StreamUrl.StringValue);
        }
    }
}