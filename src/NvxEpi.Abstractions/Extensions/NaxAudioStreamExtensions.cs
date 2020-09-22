using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.NaxAudio;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Extensions
{
    public static class NaxAudioStreamExtensions
    {
        public static void StartAudioStream(this INaxAudioRx device)
        {
            Debug.Console(1, device, "Starting NAX stream...");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
            device.Hardware.DmNaxRouting.DmNaxReceive.StartStream();
        }

        public static void StopAudioStream(this INaxAudioRx device)
        {
            Debug.Console(1, device, "Stopping NAX stream...");
            device.Hardware.DmNaxRouting.DmNaxReceive.StopStream();
        }

        public static void SetAudioAddress(this INaxAudioRx device, string address)
        {
            if (String.IsNullOrEmpty(address))
                return;

            Debug.Console(1, device, "Setting NAX stream address : '{0}", address);
            device.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
        }

        public static void Route(this INaxAudioRx rx, int txVirtualId)
        {
            if (txVirtualId == 0)
                rx.StopAudioStream();
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
                rx.StopAudioStream();
                return;
            }

            Debug.Console(1, rx, "Making an awesome NAX Audio Route : '{0}'", tx.Name);
            rx.SetAudioAddress(tx.NaxAudioTxAddress.StringValue);

            rx.StartAudioStream();
        }
    }
}