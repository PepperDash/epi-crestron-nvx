using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using NvxEpi.Device.Models.Aggregates;
using NvxEpi.Device.Models.Entities;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class VideoStreamRoutingExtensions
    {
        public static void StartVideoStream(this IHasVideoStreamRouting device)
        {
            if (device.IsTransmitter.BoolValue || device.IsStreamingVideo.BoolValue)
                return;

            device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
            device.Hardware.Control.Start(); 
        }

        public static void StopVideoStream(this IHasVideoStreamRouting device)
        {
            if (device.IsTransmitter.BoolValue || !device.IsStreamingVideo.BoolValue)
                return;

            device.Hardware.Control.Stop();
        }

        public static void SetStreamUrl(this IHasVideoStreamRouting device, string url)
        {
            if (device.IsTransmitter.BoolValue || String.IsNullOrEmpty(url))
                return;

            device.Hardware.Control.ServerUrl.StringValue = url;
            device.StartVideoStream();
        }

        public static void RouteVideoStream(this IHasVideoStreamRouting rx, int txVirtualId)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (txVirtualId == 0)
                rx.StopVideoStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IHasVideoStreamRouting>()
                    .Where(x => x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteVideoStream(tx);
            }
        }

        public static void RouteVideoStream(this IHasVideoStreamRouting rx, string txName)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxDeviceRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopVideoStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<IHasVideoStreamRouting>()
                .Where(t => t.IsTransmitter.BoolValue)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                rx.RouteVideoStream(txByName);
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null)
                return;

            rx.RouteVideoStream(txByKey);
        }

        public static void RouteVideoStream(this IHasVideoStreamRouting rx, IHasVideoStreamRouting tx)
        {
            if (tx == null)
            {
                rx.StopVideoStream();
                return;
            }

            if (rx.IsTransmitter.BoolValue || !tx.IsTransmitter.BoolValue)
                throw new NotSupportedException("device type");

            rx.SetStreamUrl(tx.StreamUrl.StringValue);
        }
    }
}