using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Aggregates;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class SecondaryAudioStreamExtensions
    {
        public static void StartAudioStream(this IHasSecondaryAudioStream device)
        {
            if (device.IsTransmitter.BoolValue)
                return;

            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
            device.Hardware.SecondaryAudio.Start();
        }

        public static void StopAudioStream(this IHasSecondaryAudioStream device)
        {
            if (device.IsTransmitter.BoolValue)
                return;

            device.Hardware.SecondaryAudio.Stop();
        }

        public static void SetAudioAddress(this IHasSecondaryAudioStream device, string address)
        {
            if (device.IsTransmitter.BoolValue || String.IsNullOrEmpty(address))
                return;

            device.Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public static void RouteAudioStream(this IHasSecondaryAudioStreamRouting rx, int txVirtualId)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (txVirtualId == 0)
                rx.StopAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IHasSecondaryAudioStreamRouting>()
                    .Where(x => x.IsTransmitter.BoolValue)
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteAudioStream(tx);
            }
        }

        public static void RouteAudioStream(this IHasSecondaryAudioStreamRouting rx, string txName)
        {
            if (rx.IsTransmitter.BoolValue)
                throw new NotSupportedException("route device is transmitter");

            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxDeviceRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopAudioStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<IHasSecondaryAudioStream>()
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

        public static void RouteAudioStream(this IHasSecondaryAudioStreamRouting rx, IHasSecondaryAudioStream tx)
        {
            if (tx == null)
            {
                rx.StopAudioStream();
                return;
            }

            if (rx.IsTransmitter.BoolValue || !tx.IsTransmitter.BoolValue)
                throw new NotSupportedException("device type");

            rx.SetAudioAddress(
                String.IsNullOrEmpty(tx.SecondaryAudioMulticastAddress.StringValue)
                    ? tx.MulticastAddress.StringValue
                    : tx.SecondaryAudioMulticastAddress.StringValue);

            rx.StartAudioStream();
        }
    }
}