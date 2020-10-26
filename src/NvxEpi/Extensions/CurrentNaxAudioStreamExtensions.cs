using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.NaxAudio;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public static class CurrentNaxAudioStreamExtensions
    {
        public static bool TryGetCurrentAudioRoute(this INaxAudioRx device, out INaxAudioTx result)
        {
            result = device.GetCurrentAudioRoute();
            return result != null;
        }

        public static INaxAudioTx GetCurrentAudioRoute(this INaxAudioRx device)
        {
            try
            {
                if (!device.IsStreamingNaxRx.BoolValue)
                    return null;

                if (device.Hardware.Control.ActiveVideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return null;

                if (device.Hardware.Control.ActiveAudioSourceFeedback != DmNvxControl.eAudioSource.DmNaxAudio)
                    return null;

                return DeviceManager
                    .AllDevices
                    .OfType<INaxAudioTx>()
                    .FirstOrDefault(
                        x =>
                            x.NaxAudioTxAddress.StringValue.Equals(
                                device.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue,
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