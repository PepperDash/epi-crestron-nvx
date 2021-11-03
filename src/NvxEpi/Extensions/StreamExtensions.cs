using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class StreamExtensions
    {
        public static void ClearStream(this IStreamWithHardware device)
        {
            if (device.IsTransmitter)
                return;

            Debug.Console(1, device, "Clearing stream");
            device.Hardware.Control.ServerUrl.StringValue = string.Empty;
        }

        public static void RouteStream(this IStreamWithHardware device, IStream tx)
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
                device.ClearStream();
            else
                device.SetStreamUrl(tx.StreamUrl.StringValue);
        }

        public static void SetStreamUrl(this IStreamWithHardware device, string url)
        {
            if (device.IsTransmitter)
                return;

            Debug.Console(1, device, "Setting stream: '{0}'", url);
            device.Hardware.Control.ServerUrl.StringValue = url;
            if (device.Hardware is DmNvxD3x)
            {
                Debug.Console(1, device, "Device is DmNvxE3x type, not able to route VideoSource");
            }
            else
            {
                device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
            }
        }
    }
}