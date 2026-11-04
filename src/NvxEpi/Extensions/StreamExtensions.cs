using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions;

public static class StreamExtensions
{
    // A route made before the transmitter has reported its stream URL (typically the rooms' default routes
    // right after program start, before the NVX endpoints are online) is held here and applied when that
    // URL arrives, instead of leaving the receiver cleared. Keyed by receiver; any later route, clear or
    // URL set on the same receiver cancels it, so a late URL can never override a newer choice.
    private sealed class PendingRoute
    {
        public IStream Tx;
        public EventHandler<FeedbackEventArgs> Handler;
    }

    private static readonly object pendingLock = new();
    private static readonly Dictionary<string, PendingRoute> pendingRoutes = new();

    private static void CancelPendingRoute(IStreamWithHardware device)
    {
        lock (pendingLock)
        {
            if (!pendingRoutes.TryGetValue(device.Key, out var pending))
                return;

            pending.Tx.StreamUrl.OutputChange -= pending.Handler;
            pendingRoutes.Remove(device.Key);
            device.LogDebug("Cancelled deferred route from {txName}", pending.Tx.Name);
        }
    }

    public static void ClearStream(this IStreamWithHardware device)
    {
        if (device.IsTransmitter)
            return;

        CancelPendingRoute(device);
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

        tx.StreamUrl.FireUpdate();

        device.LogDebug("Setting device stream : '{txName}'", tx.Name);

        if (!string.IsNullOrEmpty(tx.StreamUrl.StringValue))
        {
            device.SetStreamUrl(tx.StreamUrl.StringValue);
            return;
        }

        // The transmitter has no stream URL yet: clear the receiver as before, then wait for the URL.
        device.ClearStream();
        DeferRoute(device, tx);
    }

    private static void DeferRoute(IStreamWithHardware device, IStream tx)
    {
        var pending = new PendingRoute { Tx = tx };
        pending.Handler = (sender, args) =>
        {
            var url = tx.StreamUrl.StringValue;
            if (string.IsNullOrEmpty(url))
                return;

            lock (pendingLock)
            {
                if (!pendingRoutes.TryGetValue(device.Key, out var current) || current != pending)
                    return;

                tx.StreamUrl.OutputChange -= pending.Handler;
                pendingRoutes.Remove(device.Key);
            }

            device.LogInformation("Applying deferred route from {txName}: '{url}'", tx.Name, url);
            ApplyStreamUrl(device, url);
        };

        lock (pendingLock)
        {
            pendingRoutes[device.Key] = pending;
            tx.StreamUrl.OutputChange += pending.Handler;
        }

        device.LogInformation("{txName} has no stream URL yet; route deferred until it reports one", tx.Name);
    }

    public static void SetStreamUrl(this IStreamWithHardware device, string url)
    {
        if (device.IsTransmitter)
            return;

        CancelPendingRoute(device);
        ApplyStreamUrl(device, url);
    }

    private static void ApplyStreamUrl(IStreamWithHardware device, string url)
    {
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
