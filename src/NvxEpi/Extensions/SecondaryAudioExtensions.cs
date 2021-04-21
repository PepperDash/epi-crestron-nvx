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
            if (String.IsNullOrEmpty(address))
                return;

            var deviceWithHardware = device as ISecondardyAudioStreamWithHardware;
            if (deviceWithHardware == null) return;

            Debug.Console(1, device, "Setting Secondary Audio Address : '{0}'", address);

            deviceWithHardware.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
            deviceWithHardware.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
        }

        public static void ClearSecondaryStream(this ISecondaryAudioStream device)
        {
            var deviceWithHardware = device as ISecondardyAudioStreamWithHardware;
            if (deviceWithHardware == null) return;

            Debug.Console(1, device, "Clearing Secondary Audio Stream");
            deviceWithHardware.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = "0.0.0.0";
        }

        public static void RouteSecondaryAudio(this ISecondaryAudioStream device, ISecondaryAudioStream tx)
        {
            if (tx == null)
            {
                device.ClearSecondaryStream();
                return;
            }

            Debug.Console(1, device, "Routing device secondary audio stream : '{0}'", tx.Name);
            tx.SecondaryAudioAddress.FireUpdate();
            device.SetSecondaryAudioAddress(tx.SecondaryAudioAddress.StringValue);
        }
    }
}