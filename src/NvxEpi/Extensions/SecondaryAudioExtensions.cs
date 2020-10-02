using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class SecondaryAudioExtensions
    {
        public static void SetSecondaryAudioAddress(this ISecondaryAudioStream device, string address)
        {
            if (device.IsTransmitter)
                return;

            if (String.IsNullOrEmpty(address))
                return;

            Debug.Console(1, device, "Setting Secondary Audio Address : '{0}'", address);

            device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
            device.Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public static void ClearSecondaryStream(this ISecondaryAudioStream device)
        {
            if (device.IsTransmitter)
                return;

            Debug.Console(1, device, "Clearing Secondary Audio Stream");
            device.Hardware.SecondaryAudio.MulticastAddress.StringValue = null;
        }

        public static void RouteSecondaryAudio(this ISecondaryAudioStream device, ISecondaryAudioStream tx)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (tx == null)
            {
                device.ClearSecondaryStream();
                return;
            }

            if (!tx.IsTransmitter)
                throw new ArgumentException("tx");

            Debug.Console(1, device, "Routing device secondary audio stream : '{0}'", tx.Name);
            tx.SecondaryAudioAddress.FireUpdate();
            device.SetSecondaryAudioAddress(tx.SecondaryAudioAddress.StringValue);
        }
    }
}