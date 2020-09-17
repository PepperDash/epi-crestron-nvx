using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.SecondaryAudio
{
    public interface ISecondaryAudioStream : INvx35XHardware, INvxDevice
    {
        StringFeedback SecondaryAudioAddress { get; }
        BoolFeedback IsStreamingSecondaryAudio { get; }
        StringFeedback SecondaryAudioStreamStatus { get; }
    }

    public static class SecondaryAudioExtensions
    {
        public static void StartSecondaryAudio(this ISecondaryAudioStream device)
        {
            if (SecondaryAudioUtilities.ValidateSecondaryAudioStreamStart(device))
                return;
  
            Debug.Console(1, device, "Starting Secondary Audio...");
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
            if (device.IsTransmiter)
                return;

            if (String.IsNullOrEmpty(address))
                return;

            Debug.Console(1, device, "Setting Secondary Audio Address : '{0}'", address);
            device.Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public static void Route(this ISecondaryAudioStream device, int txId)
        {
            if (device.IsTransmiter)
                throw new ArgumentException("device");

            if (txId == 0)
            {
                device.StopSecondaryAudio();
                return;
            }

            var tx = DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .Where(x => x.IsTransmiter)
                .FirstOrDefault(x => x.DeviceId == txId);

            if (tx == null)
                return;

            device.Route(tx);
        }

        public static void Route(this ISecondaryAudioStream device, ISecondaryAudioStream tx)
        {
            if (device.IsTransmiter)
                throw new ArgumentException("device");

            if (!tx.IsTransmiter)
                throw new ArgumentException("tx");

            Debug.Console(1, device, "Routing stream : '{0}'", tx.Name);
            device.SetSecondaryAudioAddress(tx.SecondaryAudioAddress.StringValue);
        }

        public static bool TryGetCurrentAudioRoute(this ISecondaryAudioStream device, out ISecondaryAudioStream result)
        {
            result = device.GetCurrentAudioRoute();
            return result != null;
        }

        public static ISecondaryAudioStream GetCurrentAudioRoute(this ISecondaryAudioStream device)
        {
            try
            {
                if (!device.IsStreamingSecondaryAudio.BoolValue)
                    return null;

                if (device.Hardware.Control.ActiveVideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return null;

                if (device.Hardware.Control.ActiveAudioSourceFeedback != DmNvxControl.eAudioSource.SecondaryStreamAudio)
                    return null;

                var result = DeviceManager
                        .AllDevices
                        .OfType<ISecondaryAudioStream>()
                        .Where(t => t.IsTransmiter && t.IsStreamingSecondaryAudio.BoolValue)
                        .FirstOrDefault(
                            x =>
                                x.SecondaryAudioAddress.StringValue.Equals(
                                    device.Hardware.SecondaryAudio.MulticastAddressFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                return result;
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error getting current audio route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }

    internal class SecondaryAudioUtilities
    {
        public static bool ValidateSecondaryAudioStreamStart(ISecondaryAudioStream device)
        {
            if (!device.IsTransmiter) 
                return device.Hardware.SecondaryAudio.StartFeedback.BoolValue;

            device.Hardware.SecondaryAudio.EnableAutomaticInitiation();
            return true;
        }

        public static bool ValidateSecondaryAudioStreamStop(ISecondaryAudioStream device)
        {
            if (!device.IsTransmiter) 
                return !device.Hardware.SecondaryAudio.StartFeedback.BoolValue;

            device.Hardware.SecondaryAudio.EnableAutomaticInitiation();
            return true;
        }
    }

    public class SecondaryAudioRouter
    {
        public static void Route(int txId, int rxId)
        {
            if (rxId == 0)
                return;

            var rx = DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .Where(x => !x.IsTransmiter)
                .FirstOrDefault(x => x.DeviceId == rxId);

            if (rx == null)
                return;

            if (txId == 0)
            {
                rx.StopSecondaryAudio();
                return;
            }

            rx.Route(txId);
        }
    }
}