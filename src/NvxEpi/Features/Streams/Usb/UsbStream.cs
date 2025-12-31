using System;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Extensions;
using NvxEpi.Features.Config;
using NvxEpi.Services.Feedback;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Usb;

public class UsbStream : IUsbStreamWithHardware
{
    public static IUsbStreamWithHardware GetUsbStream(
        INvxDeviceWithHardware device,
        NvxUsbProperties incomingProps
    )
    {
        try
        {
            var props =
                incomingProps
                ?? new NvxUsbProperties()
                {
                    Mode = "local",
                    Default = string.Empty,
                    FollowVideo = false,
                    IsLayer3 = false,
                };

            device.LogDebug(
                "Mode : {mode}, Default : {default}, FollowVideo = {followVideo}",
                props.Mode,
                props.Default,
                props.FollowVideo
            );

            if (string.IsNullOrEmpty(props.Mode))
                props.Mode = "local";

            if (string.IsNullOrEmpty(props.Default))
                props.Default = string.Empty;

            return new UsbStream(
                device,
                !props.Mode.Equals("local", StringComparison.OrdinalIgnoreCase),
                props.FollowVideo,
                props.Default,
                props.IsLayer3
            );
        }
        catch (Exception ex)
        {
            device.LogError("Exception in GetUsbStream: {message}", ex.Message);
            device.LogDebug(ex, "Stack Trace: ");
            return null;
        }
    }

    private readonly INvxDeviceWithHardware _device;
    private readonly StringFeedback _usbLocalId;
    private readonly ReadOnlyDictionary<uint, StringFeedback> _usbRemoteIds;
    private readonly bool _isRemote;

    private UsbStream(
        INvxDeviceWithHardware device,
        bool isRemote,
        bool followStream,
        string defaultPair,
        bool isLayer3
    )
    {
        _device = device;
        _isRemote = isRemote;
        _usbLocalId = UsbLocalAddressFeedback.GetFeedback(device.Hardware);
        _usbRemoteIds = UsbRemoteAddressFeedback.GetFeedbacks(device.Hardware);

        device.Feedbacks.AddRange(
            new Feedback[]
            {
                _usbLocalId,
                new BoolFeedback("UsbFollowsVideoStream", () => !followStream || IsTransmitter),
                UsbRemoteAddressFeedback.GetFeedback(device.Hardware),
                UsbModeFeedback.GetFeedback(Hardware),
                UsbStatusFeedback.GetFeedback(Hardware),
                UsbRouteFeedback.GetFeedback(device.Hardware),
            }
        );

        foreach (var item in _usbRemoteIds.Values)
            _device.Feedbacks.Add(item);

        Hardware.OnlineStatusChange += (currentDevice, args) =>
        {
            if (!args.DeviceOnLine || Hardware.UsbInput == null)
                return;

            Hardware.UsbInput.AutomaticUsbPairingEnabled();
            Hardware.UsbInput.Mode = IsRemote
                ? DmNvxUsbInput.eUsbMode.Remote
                : DmNvxUsbInput.eUsbMode.Local;
            Hardware.UsbInput.TransportMode = isLayer3
                ? DmNvxUsbInput.eUsbTransportMode.Layer3
                : DmNvxUsbInput.eUsbTransportMode.Layer2;
            if (!followStream || IsTransmitter)
                return;

            SetDefaultStream(isRemote, defaultPair);
        };

        if (Hardware.UsbInput == null)
        {
            return;
        }

        Hardware.UsbInput.UsbInputChange += UsbInput_UsbInputChange;

        Hardware.UsbInput.AutomaticUsbPairingEnabled();

        Hardware.OnlineStatusChange += (currentDevice, args) =>
        {
            if (!args.DeviceOnLine || Hardware.UsbInput == null)
            {
                return;
            }

            foreach (var sig in Hardware.UsbInput.RemoteDeviceIds.Values)
            {
                sig.StringValue = UsbStreamExt.ClearUsbValue;
            }
        };

        if (!followStream || IsTransmitter)
        {
            device.LogDebug("Will not Follow Stream!");
            return;
        }

        if (device is not ICurrentStream stream)
        {
            return;
        }

        stream.StreamUrl.OutputChange += (sender, args) => FollowCurrentRoute(args.StringValue);
    }

    void UsbInput_UsbInputChange(
        object sender,
        Crestron.SimplSharpPro.DeviceSupport.GenericEventArgs args
    )
    {
        if (args.EventId == UsbInputEventIds.RemoteDeviceIdFeedbackEventId)
        {
            var feedback = _device.Feedbacks.FirstOrDefault(o => o.Key == UsbRouteFeedback.Key);
            feedback.FireUpdate();
        }
    }

    public void MakeUsbRoute(IUsbStreamWithHardware hardware)
    {
        if (hardware == null || hardware.Hardware.UsbInput == null)
        {
            this.LogWarning("Unable to make USB Route - hardware is null");
            return;
        }

        this.LogDebug("Trying USB Route {localId}", hardware.UsbLocalId.StringValue);

        if (string.IsNullOrEmpty(hardware.UsbLocalId.StringValue))
            return;

        // device is remote, hardware is local
        if (IsRemote && !hardware.IsRemote)
        {
            this.LogDebug("Routing to Local from New Route : {name}", hardware.Name);

            hardware.AddRemoteUsbStreamToLocal(this);
            return;
        }

        // device is local, hardware is remote
        if (!IsRemote && hardware.IsRemote)
        {
            this.LogDebug("Routing to Remote from New Route : {name}", hardware.Name);

            this.AddRemoteUsbStreamToLocal(hardware);
            return;
        }

        this.LogDebug("Cannot route usb to device : {name}", hardware.Key);
    }

    private void FollowCurrentRoute(string streamUrl)
    {
        if (string.IsNullOrEmpty(streamUrl))
        {
            ClearCurrentUsbRoute();
            return;
        }

        var currentRoute =
            DeviceManager
                .AllDevices.OfType<IStreamWithHardware>()
                .FirstOrDefault(x =>
                {
                    if (x.StreamUrl == null)
                    {
                        this.LogError("StreamUrl Is Null!");
                        return false;
                    }
                    if (string.IsNullOrEmpty(x.StreamUrl.StringValue))
                    {
                        this.LogError("StreamUrl Is Empty!");
                        return false;
                    }
                    this.LogDebug("StreamUrl Is Valid!");

                    return x.IsTransmitter && x.StreamUrl.StringValue.Equals(streamUrl);
                }) as IUsbStreamWithHardware;

        if (currentRoute == null)
        {
            this.LogDebug("No Current Route Found for streamUrl : {0}", streamUrl);
            ClearCurrentUsbRoute();
            return;
        }

        this.LogInformation(
            "Following Current Route : {0} for streamUrl : {1}",
            currentRoute.Name,
            streamUrl
        );

        if (IsRemote && !currentRoute.IsRemote)
        {
            this.LogDebug("Routing to Local from CurrentRoute : {0}!", currentRoute.Name);

            currentRoute.AddRemoteUsbStreamToLocal(this);
        }
        else if (!IsRemote && currentRoute.IsRemote)
        {
            this.LogDebug("Routing to Remote from CurrentRoute : {0}!", currentRoute.Name);

            this.AddRemoteUsbStreamToLocal(currentRoute);
        }
        else
            this.LogError("Cannot follow usb on device : {0}", currentRoute.Key);
    }

    public void ClearCurrentUsbRoute()
    {
        if (Hardware.UsbInput.Mode == DmNvxUsbInput.eUsbMode.Local)
        {
            this.LogInformation("Skipping ClearCurrentUsbRoute - Device is local");
            return;
        }

        this.RemoveRemoteUsbFromPairedLocal();
        Hardware.UsbInput.RemoteDeviceIds[1].StringValue = UsbStreamExt.ClearUsbValue;
    }

    public void ClearRemoteUsbRoute()
    {
        foreach (
            var item in DeviceManager
                .AllDevices.OfType<IUsbStreamWithHardware>()
                .Where(device =>
                    device.Hardware.UsbInput.RemoteDeviceIdFeedbacks.Values.Any(y =>
                        y.StringValue.Equals(Hardware.UsbInput.LocalDeviceIdFeedback.StringValue)
                    )
                )
        )
        {
            item.Hardware.UsbInput.RemoteDeviceIds[1].StringValue = UsbStreamExt.ClearUsbValue;

            foreach (var id in item.Hardware.UsbInput.RemoteDeviceIds)
            {
                if (
                    !id.StringValue.Equals(
                        UsbStreamExt.ClearUsbValue,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    id.StringValue = UsbStreamExt.ClearUsbValue;
                }
            }
            if (item.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                item.Hardware.UsbInput.RemovePairing();
        }
    }

    private void SetDefaultStream(bool isRemote, string defaultPair)
    {
        if (!isRemote || string.IsNullOrEmpty(defaultPair))
            return;

        if (DeviceManager.GetDeviceForKey(defaultPair) is not IUsbStreamWithHardware local)
            return;

        local.AddRemoteUsbStreamToLocal(this);
    }

    public string Key
    {
        get { return _device.Key; }
    }

    public RoutingPortCollection<RoutingInputPort> InputPorts
    {
        get { return _device.InputPorts; }
    }

    public RoutingPortCollection<RoutingOutputPort> OutputPorts
    {
        get { return _device.OutputPorts; }
    }

    public FeedbackCollection<Feedback> Feedbacks
    {
        get { return _device.Feedbacks; }
    }

    public IntFeedback DeviceMode
    {
        get { return _device.DeviceMode; }
    }

    public bool IsTransmitter
    {
        get { return _device.IsTransmitter; }
    }

    public string Name
    {
        get { return _device.Name; }
    }

    public int DeviceId
    {
        get { return _device.DeviceId; }
    }

    public bool IsRemote
    {
        get { return _isRemote; }
    }

    public StringFeedback UsbLocalId
    {
        get { return _usbLocalId; }
    }

    public ReadOnlyDictionary<uint, StringFeedback> UsbRemoteIds
    {
        get { return _usbRemoteIds; }
    }

    public DmNvxBaseClass Hardware
    {
        get { return _device.Hardware; }
    }

    public BoolFeedback IsOnline
    {
        get { return _device.IsOnline; }
    }
}
