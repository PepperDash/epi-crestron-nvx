using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using PepperDash.Core;

namespace NvxEpi.Extensions;

public static class SecondaryAudioExtensions
{
    public const string NoRouteString = "0.0.0.0";

    public static void SetSecondaryAudioAddress(this ISecondaryAudioStream device, string address)
    {
        if (string.IsNullOrEmpty(address))
        {
            Debug.LogWarning(device, "Secondary Audio Address null or empty");
            return;
        }

        if (device is not ISecondaryAudioStreamWithHardware deviceWithHardware) return;

        Debug.LogInformation(device, "Setting Secondary Audio Address : '{0}'", address);

        if (deviceWithHardware.Hardware.DmNaxRouting.DmNaxTransmit.MulticastAddressFeedback.StringValue == address && address != NoRouteString)
        {
            Debug.LogWarning(device, "Secondary Audio Address is same as this unit's Tx address: '{0}'", address);
            deviceWithHardware.Hardware.Control.AudioSource = deviceWithHardware.Hardware.Control.DmNaxAudioSourceFeedback;
        }
        else
        {
            deviceWithHardware.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
            deviceWithHardware.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;   
        }
    }

    public static void ClearSecondaryStream(this ISecondaryAudioStream device)
    {
        if (device is not ISecondaryAudioStreamWithHardware) return;

        Debug.LogInformation(device, "Clearing Secondary Audio Stream");
        device.SetSecondaryAudioAddress(NoRouteString);
    }

    public static void RouteSecondaryAudio(this ISecondaryAudioStream device, ISecondaryAudioStream tx)
    {
        if (tx == null)
        {
            device.ClearSecondaryStream();
            return;
        }

        Debug.LogInformation(device, "Routing device secondary audio stream : '{0}'", tx.Name);
        tx.TxAudioAddress.FireUpdate();
        device.SetSecondaryAudioAddress(tx.TxAudioAddress.StringValue);
    }
}