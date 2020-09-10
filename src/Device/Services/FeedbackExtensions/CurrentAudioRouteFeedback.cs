using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class CurrentAudioRouteFeedback
    {
        public static readonly string NameKey = DeviceFeedbackEnum.CurrentAudioRoute.ToString();
        public static readonly string ValueKey = DeviceFeedbackEnum.CurrentAudioRouteValue.ToString();

        public static bool TryGetCurrentAudioRoute(this ISecondaryAudioStream device, out INvxDevice result)
        {
            try
            {
                result = null;
                if (!device.IsStreamingAudio.BoolValue)
                    return false;

                if (device.Hardware.Control.ActiveVideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return false;

                if (device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input1
                    || device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.Input2)
                    return false;

                if (device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.SecondaryStreamAudio)
                {
                    result = DeviceManager
                        .AllDevices
                        .OfType<ISecondaryAudioStreamRouting>()
                        .Where(t => t.IsTransmitter.BoolValue && t.IsStreamingAudio.BoolValue)
                        .FirstOrDefault(
                            x =>
                                x.AudioMulticastAddress.StringValue.Equals(
                                    device.Hardware.SecondaryAudio.MulticastAddressFeedback.StringValue,
                                    StringComparison.OrdinalIgnoreCase));

                    return result != null;
                }

                if (device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.DanteAes67Audio ||
                    device.Hardware.Control.ActiveAudioSourceFeedback == DmNvxControl.eAudioSource.DmNaxAudio)
                {
                    throw new NotSupportedException(device.Hardware.Control.ActiveAudioSourceFeedback.ToString());
                }

                return false;
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error getting current audio route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }
}