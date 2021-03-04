using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.NaxAudio;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public static class NaxAudioStreamExtensions
    {
        public static void ClearAudioStream(this INaxAudioRx device)
        {
            var deviceWithHardware = device as INaxAudioRxWithHardware;
            if (deviceWithHardware == null)
                return;

            Debug.Console(1, device, "Stopping NAX stream...");
            deviceWithHardware.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = String.Empty;
        }

        public static void SetAudioAddress(this INaxAudioRx device, string address)
        {
            if (String.IsNullOrEmpty(address))
                device.ClearAudioStream();

            var deviceWithHardware = device as INaxAudioRxWithHardware;
            if (deviceWithHardware == null)
                return;

            Debug.Console(1, device, "Setting NAX stream address : '{0}", address);
            deviceWithHardware.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
            deviceWithHardware.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
        }

        public static void Route(this INaxAudioRx rx, int txVirtualId)
        {
            if (txVirtualId == 0)
                rx.ClearAudioStream();
            else
            {
                var tx = DeviceManager
                    .AllDevices
                    .OfType<INaxAudioTx>()
                    .FirstOrDefault(x => x.DeviceId == txVirtualId);

                if (tx == null)
                    return;

                rx.Route(tx);
            }
        }

        public static void Route(this INaxAudioRx rx, INaxAudioTx tx)
        {
            if (tx == null)
            {
                rx.ClearAudioStream();
                return;
            }

            Debug.Console(1, rx, "Making an awesome NAX Audio Route : '{0}'", tx.Name);
            rx.SetAudioAddress(tx.NaxAudioTxAddress.StringValue);
        }
    }
}