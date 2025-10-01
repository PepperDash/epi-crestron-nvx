using System;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Devices;
using NvxEpi.Extensions;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback;

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
            if (a.EventId != UsbInputEventIds.RemoteDeviceIdFeedbackEventId && a.EventId != UsbInputEventIds.PairedEventId) return;

            // NVX Routing via the plugin does not currently support more than one remote/local USB device pair. Essentials is only concerned with the first remote ID
            if (a.Index > 1)
            {
                return;
            }

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

        var remoteDeviceId = string.IsNullOrEmpty(device.UsbInput.RemoteDeviceIdFeedback.StringValue)
            ? "00"
            : device.UsbInput.RemoteDeviceIdFeedback.StringValue;

        Debug.LogVerbose("Current USB remote ID: {remoteDeviceId}", remoteDeviceId);
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

        var sb = new StringBuilder();
        sb.AppendFormat("Device ID: {0}" + Newline, remoteEndpoint.DeviceId);
        sb.AppendFormat("IsTransmitter: {0}" + Newline, remoteEndpoint.IsTransmitter);
        sb.AppendFormat("IP Address: {0}" + Newline, remoteEndpoint.Hardware.Network.IpAddressFeedback.StringValue);
        sb.AppendFormat("Local USB Address: {0}" + Newline, remoteEndpoint.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue);
        sb.AppendFormat("Remote USB Address: {0}" + Newline, remoteEndpoint.Hardware.UsbInput.RemoteDeviceIdFeedback.StringValue);

        remoteEndpoint.LogVerbose(sb.ToString());

        var macAddress = remoteEndpoint.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;
        //var isTransmitter = remoteEndpoint.Hardware.Control.DeviceModeFeedback == eDeviceMode.Transmitter;
        var isTransmitter = remoteEndpoint.IsTransmitter;

        var deviceId = remoteEndpoint.DeviceId + (isTransmitter ? 0 : 1000);

        return deviceId;
    }
}