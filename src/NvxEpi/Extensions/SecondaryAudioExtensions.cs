using System;
using System.Linq;
using NvxEpi.Abstractions.SecondaryAudio;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public static class SecondaryAudioExtensions
    {
        public static void RouteSecondaryAudio(this ISecondaryAudioStream device, ISecondaryAudioStream tx)
        {
            if (device.IsTransmitter)
                throw new ArgumentException("device");

            if (tx == null)
            {
                device.ClearSecondaryAudioStream();
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