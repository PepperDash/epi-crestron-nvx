using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class SecondaryAudioStreamRoutingExtensions
    {
        public static void StartAudioStream(this ISecondaryAudioStream device)
        {
            if (device.IsTransmitter.BoolValue)
                return;

            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
            device.Hardware.SecondaryAudio.Start();
        }

        public static void StopAudioStream(this ISecondaryAudioStream device)
        {
            if (device.IsTransmitter.BoolValue)
                return;

            device.Hardware.SecondaryAudio.Stop();
        }

        public static void SetAudioAddress(this ISecondaryAudioStream device, string address)
        {
            if (device.IsTransmitter.BoolValue || String.IsNullOrEmpty(address))
                return;

            device.Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public static void RouteAudioStream(this ISecondaryAudioStreamRouting rx, int txVirtualId)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (txVirtualId == 0)
                rx.StopAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<ISecondaryAudioStreamRouting>()
                    .Where(x => x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteAudioStream(tx);
            }
        }

        public static void RouteAudioStream(this ISecondaryAudioStreamRouting rx, string txName)
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
                .OfType<ISecondaryAudioStream>()
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

        public static void RouteAudioStream(this ISecondaryAudioStreamRouting rx, ISecondaryAudioStream tx)
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

            rx.SetAudioAddress(
                String.IsNullOrEmpty(tx.AudioMulticastAddress.StringValue)
                    ? tx.MulticastAddress.StringValue
                    : tx.AudioMulticastAddress.StringValue);

            rx.StartAudioStream();
        }
    }
}