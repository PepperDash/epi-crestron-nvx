using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class CurrentVideoRouteFeedback
    {
        public static readonly string NameKey = "CurrentVideoRoute";
        public static readonly string ValueKey = "CurrentVideoRouteValue";

        public static bool TryGetCurrentVideoRoute(this INvxDevice device, out INvxDevice result)
        {
            try
            {
                result = null;
                if (device.IsTransmitter.BoolValue || device.VirtualDeviceId == default(int) ||
                    !device.IsStreamingVideo.BoolValue)
                    return false;

                if (device.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi1 ||
                    device.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Hdmi2 ||
                    device.Hardware.Control.VideoSourceFeedback == eSfpVideoSourceTypes.Disable)
                    return false;

                result = DeviceManager
                    .AllDevices
                    .OfType<IVideoStreamRouting>()
                    .Where(t => t.IsTransmitter.BoolValue && t.IsStreamingVideo.BoolValue)
                    .FirstOrDefault(
                        x =>
                            x.StreamUrl.StringValue.Equals(device.StreamUrl.StringValue,
                                StringComparison.OrdinalIgnoreCase));

                return result != null;
            }
            catch (Exception ex)
            {
                Debug.Console(1, device, "Error getting current video route : {0}\r{1}\r{2}", ex.Message, ex.InnerException, ex.StackTrace);
                throw;
            }
        }
    }
}