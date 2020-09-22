using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Extensions
{
    public static class SecondaryAudioExtensions
    {
        public static void StartSecondaryAudio(this ISecondaryAudioStream device)
        {
            if (SecondaryAudioUtilities.ValidateSecondaryAudioStreamStart(device))
                return;
  
            Debug.Console(1, device, "Starting Secondary Audio...");
            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
            device.Hardware.SecondaryAudio.DisableAutomaticInitiation();
            device.Hardware.SecondaryAudio.Start();
        }

        public static void StopSecondaryAudio(this ISecondaryAudioStream device)
        {
            if (SecondaryAudioUtilities.ValidateSecondaryAudioStreamStop(device))
                return;

            Debug.Console(1, device, "Starting Secondary Audio...");
            device.Hardware.SecondaryAudio.DisableAutomaticInitiation();
            device.Hardware.SecondaryAudio.Start();
        }

        public static void SetSecondaryAudioAddress(this ISecondaryAudioStream device, string address)
        {
            if (device.IsTransmitter)
                return;

            if (String.IsNullOrEmpty(address))
                return;

            Debug.Console(1, device, "Setting Secondary Audio Address : '{0}'", address);

            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
            device.StartSecondaryAudio();
            device.Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public static void RouteSecondaryAudio(this ISecondaryAudioStream device, ushort txId)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (txId == 0)
            {
                device.StopSecondaryAudio();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .Where(x => x.IsTransmitter)
                .FirstOrDefault(x => x.DeviceId == txId);

            if (tx == null)
                return;

            device.RouteSecondaryAudio(tx);
        }

        public static void RouteSecondaryAudio(this ISecondaryAudioStream device, ISecondaryAudioStream tx)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (!tx.IsTransmitter)
                throw new ArgumentException("tx");

            Debug.Console(1, device, "Routing stream : '{0}'", tx.Name);
            device.SetSecondaryAudioAddress(tx.SecondaryAudioAddress.StringValue);
        }
    }
}