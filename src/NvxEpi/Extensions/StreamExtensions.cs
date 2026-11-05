using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions;

public static class StreamExtensions
{
    // A route made before the transmitter has reported its stream URL (typically the rooms' default routes
    // right after program start, before the NVX endpoints are online) is held here and applied when that
    // URL arrives, instead of leaving the receiver cleared. Keyed by receiver; any later route, clear,
    // URL set or explicit local/automatic/none input selection on the same receiver cancels it, so a late URL can never override a newer choice.
    // pendingLock serializes each cancellation/check with the receiver write that follows it, on every
    // path (clear, set, deferred apply), so a stale callback cannot write after a newer route has won.
    private sealed class PendingRoute
    {
        public IStream Tx;
        public EventHandler<FeedbackEventArgs> Handler;
    }

    private static readonly object pendingLock = new();
    private static readonly Dictionary<string, PendingRoute> pendingRoutes = new();

    /// <summary>
    /// Runs an explicit non-stream input selection (a local input, automatic or none) on a receiver:
    /// cancels its pending deferred route and makes the selection under the same lock, so a late
    /// stream URL can't switch the receiver back to Stream over it.
    /// </summary>
    internal static void SelectInputCancellingPendingRoute(INvxDevice device, Action select)
    {
        lock (pendingLock)
        {
            CancelPendingRoute(device);
            select();
        }
    }

    private static void CancelPendingRoute(INvxDevice device)
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

        lock (pendingLock)
        {
            CancelPendingRoute(device);
            device.LogDebug("Clearing stream");
            device.Hardware.Control.ServerUrl.StringValue = string.Empty;
        }
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

            // Check and write under the lock: a newer clear/set either ran first and removed this entry
            // (so this returns), or waits until this write is done and then overrides it.
            lock (pendingLock)
            {
                if (!pendingRoutes.TryGetValue(device.Key, out var current) || current != pending)
                    return;

                tx.StreamUrl.OutputChange -= pending.Handler;
                pendingRoutes.Remove(device.Key);

                device.LogInformation("Applying deferred route from {txName}: '{url}'", tx.Name, url);
                ApplyStreamUrl(device, url);
            }
        };

        // Clear and register in one step, so no newer route can land between them.
        lock (pendingLock)
        {
            device.ClearStream(); // re-enters pendingLock (Monitor is reentrant); cancels any older pending route

            pendingRoutes[device.Key] = pending;
            tx.StreamUrl.OutputChange += pending.Handler;
        }

        device.LogInformation("{txName} has no stream URL yet; route deferred until it reports one", tx.Name);

        // The URL may have arrived after RouteStream's check but before the handler was subscribed;
        // that change was not delivered, so check once now. The handler is a no-op if the URL is still
        // empty or the route was already applied or cancelled.
        pending.Handler(tx, null);
    }

    public static void SetStreamUrl(this IStreamWithHardware device, string url)
    {
        if (device.IsTransmitter)
            return;

        lock (pendingLock)
        {
            CancelPendingRoute(device);
            ApplyStreamUrl(device, url);
        }
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
