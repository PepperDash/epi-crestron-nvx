using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public static class CurrentSecondaryAudioStreamExtensions
    {
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

                return DeviceManager
                    .AllDevices
                    .OfType<ISecondaryAudioStream>()
                    .Where(t => t.IsTransmitter && t.IsStreamingSecondaryAudio.BoolValue)
                    .FirstOrDefault(
                        x =>
                            x.SecondaryAudioAddress.StringValue.Equals(
                                device.Hardware.SecondaryAudio.MulticastAddressFeedback.StringValue,
                                StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error getting current audio route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }
}