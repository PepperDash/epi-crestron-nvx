using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models.Aggregates;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.DeviceExtensions
{
    public static class NaxAudioStreamExtensions
    {
        public static void StartAudioStream(this IHasNaxAudioRxStream device)
        {
            Debug.Console(1, device, "Starting NAX stream...");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
            device.Hardware.DmNaxRouting.DmNaxReceive.StartStream();
        }

        public static void StopAudioStream(this IHasNaxAudioRxStream device)
        {
            Debug.Console(1, device, "Stopping NAX stream...");
            device.Hardware.DmNaxRouting.DmNaxReceive.StopStream();
        }

        public static void SetAudioAddress(this IHasNaxAudioRxStream device, string address)
        {
            if (String.IsNullOrEmpty(address))
                return;

            Debug.Console(1, device, "Setting NAX stream address : '{0}", address);
            device.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
        }

        public static void RouteAudioStream(this IHasHasNaxAudioStreamRouting rx, int txVirtualId)
        {
            if (txVirtualId == 0)
                rx.StopAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<IHasNaxAudioTxStream>()
                    .FirstOrDefault(x => x.VirtualDeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.RouteAudioStream(tx);
            }
        }

        public static void RouteAudioStream(this IHasHasNaxAudioStreamRouting rx, string txName)
        {
            if (String.IsNullOrEmpty(txName))
                return;

            if (txName.Equals(NvxDeviceRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
            {
                rx.StopAudioStream();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<IHasNaxAudioTxStream>()
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

        public static void RouteAudioStream(this IHasHasNaxAudioStreamRouting rx, IHasNaxAudioTxStream tx)
        {
            if (tx == null)
            {
                rx.StopAudioStream();
                return;
            }

            Debug.Console(1, rx, "Making an awesome NAX Audio Route : '{0}'", tx.Name);
            rx.SetAudioAddress(
                String.IsNullOrEmpty(tx.NaxAudioTxMulticastAddress.StringValue)
                    ? tx.MulticastAddress.StringValue
                    : tx.NaxAudioTxMulticastAddress.StringValue);

            rx.StartAudioStream();
        }
    }
}