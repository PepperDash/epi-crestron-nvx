using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using PepperDash.Core.Logging;

namespace NvxEpi.Extensions;

public static class StreamExtensions
{
    public static void ClearStream(this IStreamWithHardware device)
    {
        if (device.IsTransmitter)
            return;

        device.LogDebug("Clearing stream");
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

        device.LogDebug("Routing device stream : '{0}'", tx.Name);
        tx.StreamUrl.FireUpdate();

        if (string.IsNullOrEmpty(tx.StreamUrl.StringValue))
            device.ClearStream();
        else
            device.SetStreamUrl(tx.StreamUrl.StringValue);
    }

    public static void SetStreamUrl(this IStreamWithHardware device, string url)
    {
        if (device.IsTransmitter)
            return;

        device.LogDebug("Setting stream: '{0}'", url);
        device.Hardware.Control.ServerUrl.StringValue = url;
        if (device.Hardware is DmNvxD3x)
        {
            device.LogDebug("Device is DmNvxE3x type, not able to route VideoSource");
        }
        else
        {
            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
        }
    }
}
