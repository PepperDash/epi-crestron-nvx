using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class StreamExtensions
    {
        public static void ClearStream(this IStream device)
        {
            if (device.IsTransmitter)
                return;

            var deviceWithHardware = device as IStreamWithHardware;
            if (deviceWithHardware == null)
                return;
            
            Debug.Console(1, device, "Clearing stream");
            deviceWithHardware.Hardware.Control.ServerUrl.StringValue = string.Empty;
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
            tx.StreamUrl.FireUpdate();

            if (String.IsNullOrEmpty(tx.StreamUrl.StringValue))
                return;

            device.SetStreamUrl(tx.StreamUrl.StringValue);
        }

        public static void SetStreamUrl(this IStream device, string url)
        {
            if (device.IsTransmitter)
                return;

            if (String.IsNullOrEmpty(url))
                return;

            var deviceWithHardware = device as IStreamWithHardware;
            if (deviceWithHardware == null)
                return;

            Debug.Console(1, device, "Setting stream: '{0}'", url);
            deviceWithHardware.Hardware.Control.ServerUrl.StringValue = url;
            deviceWithHardware.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }
    }
}