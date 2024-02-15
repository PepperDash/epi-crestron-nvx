using System;
using System.Linq;
using System.Net;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronXml;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using Newtonsoft.Json.Schema;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Devices;
using NvxEpi.Extensions;
using NvxEpi.Features.Routing;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public static class UsbRouteFeedback
    {
        public const string Key = "UsbRoute";
        
        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.UsbInput == null)
                return new IntFeedback(Key, () => 0);

            var usbRoute = ReturnRoute(device);
            var usbRouteFb = new IntFeedback(Key, () => usbRoute);

            device.UsbInput.UsbInputChange += (s, a) =>
            {
                if (a.EventId != UsbInputEventIds.RemoteDeviceIdFeedbackEventId) return;
                usbRoute = ReturnRoute(device);
                usbRouteFb.FireUpdate();
            };

            return usbRouteFb;
        }

        private static readonly string Newline = CrestronEnvironment.NewLine;

        private static int ReturnRoute(DmNvxBaseClass device)
        {
            if (device == null || device.UsbInput == null)
            {
                return 0;
            }

            var deviceIp = device.Network.IpAddressFeedback.StringValue;

            var remoteDeviceId = String.IsNullOrEmpty(device.UsbInput.RemoteDeviceIdFeedback.StringValue)
                ? "00"
                : device.UsbInput.RemoteDeviceIdFeedback.StringValue;

            Debug.Console(2, "{1} :: remoteDeviceId = {0}", remoteDeviceId, deviceIp);
            if (remoteDeviceId.Equals(UsbStreamExt.ClearUsbValue)) return 0;

            var remoteEndpoint = DeviceManager.AllDevices.OfType<NvxBaseDevice>()
                .Where(d => d.Hardware.UsbInput != null)
                .FirstOrDefault(
                    o =>
                        o.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue.Equals(remoteDeviceId,
                            StringComparison.OrdinalIgnoreCase));

            if (remoteEndpoint == null)
            {
                return 0;
            }

            var sb = new StringBuilder(Newline);
            sb.AppendFormat("Device ID :: {0}" + Newline, remoteEndpoint.DeviceId);
            sb.AppendFormat("IsTransmitter :: {0}" + Newline, remoteEndpoint.IsTransmitter);
            sb.AppendFormat("IP Address :: {0}" + Newline, remoteEndpoint.Hardware.Network.IpAddressFeedback.StringValue);
            sb.AppendFormat("Local USB Address :: {0}" + Newline, remoteEndpoint.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue);
            sb.AppendFormat("Remote USB Address :: {0}" + Newline, remoteEndpoint.Hardware.UsbInput.RemoteDeviceIdFeedback.StringValue);

            Debug.Console(2, remoteEndpoint, sb.ToString());


            var macAddress = remoteEndpoint.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;
            //var isTransmitter = remoteEndpoint.Hardware.Control.DeviceModeFeedback == eDeviceMode.Transmitter;
            var isTransmitter = remoteEndpoint.IsTransmitter;

            var deviceId = remoteEndpoint.DeviceId + (isTransmitter ? 0 : 1000);

            return deviceId;
        }
    }
}