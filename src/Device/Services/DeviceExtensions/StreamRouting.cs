﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class StreamRoutingExtensions
    {
        public static void StartVideoStream(this INvxDevice device)
        {
            if (device.IsTransmitter.BoolValue || device.IsStreamingVideo.BoolValue)
                return;

            device.Hardware.Control.Start();
        }

        public static void StopVideoStream(this INvxDevice device)
        {
            if (device.IsTransmitter.BoolValue || !device.IsStreamingVideo.BoolValue)
                return;

            device.Hardware.Control.Stop();
        }

        public static void SetStreamUrl(this INvxDevice device, string url)
        {
            if (device.IsTransmitter.BoolValue || String.IsNullOrEmpty(url))
                return;

            device.Hardware.Control.ServerUrl.StringValue = url;
        }

        public static void RouteVideoStream(this IVideoStreamRouting rx, int txVirtualId)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (txVirtualId == 0)
                rx.StopVideoStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IVideoStreamRouting>()
                    .Where(x => x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteVideoStream(tx);
            }
        }

        public static void RouteVideoStream(this INvxDevice rx, string txName)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopVideoStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<IVideoStreamRouting>()
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

        public static void RouteVideoStream(this INvxDevice rx, INvxDevice tx)
        {
            if (tx == null)
            {
                rx.StopVideoStream();
                return;
            }

            if (rx.IsTransmitter.BoolValue || !tx.IsTransmitter.BoolValue)
                throw new NotSupportedException("device type");
            
            if (rx == null)
                throw new NullReferenceException("rx");

            rx.SetStreamUrl(tx.StreamUrl.StringValue);
            var videoInputSwitcher = rx as IVideoInputSwitcher;
            if (videoInputSwitcher != null)
                videoInputSwitcher.SetInput(eSfpVideoSourceTypes.Stream);

            rx.StartVideoStream();
        }


        public static void RouteAudioStream(this IAudioStreamRouting rx, int txVirtualId)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (txVirtualId == 0)
                rx.StopAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IAudioStreamRouting>()
                    .Where(x => x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;
 
                rx.RouteAudioStream(tx);
            }
        }

        public static void RouteAudioStream(this IAudioStreamRouting rx, string txName)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopAudioStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<IAudioStream>()
                .Where(t => t.IsTransmitter.BoolValue)
                .ToList();

            var txByName = tx.FirstOrDefault(x => x.Name.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByName != null)
            {
                rx.RouteAudioStream(txByName);
                return;
            }

            var txByKey = tx.FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));
            if (txByKey == null)
                return;

            rx.RouteAudioStream(txByKey);
        }

        public static void RouteAudioStream(this IAudioStreamRouting rx, IAudioStream tx)
        {
            if (tx == null)
            {
                rx.StopAudioStream();
                return;
            }

            if (rx.IsTransmitter.BoolValue || !tx.IsTransmitter.BoolValue)
                throw new NotSupportedException("device type");

            if (rx == null)
                throw new NullReferenceException("rx");

            var rxWithInput = rx as IAudioInputSwitcher;
            if (rxWithInput != null)
                rxWithInput.SetInput(AudioInputEnum.SecondaryStream);

            rx.SetAudioAddress(
                String.IsNullOrEmpty(tx.AudioMulticastAddress.StringValue)
                    ? tx.MulticastAddress.StringValue
                    : tx.AudioMulticastAddress.StringValue);

            rx.StartAudioStream();
        }
    }
}