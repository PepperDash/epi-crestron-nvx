using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Endpoints;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing;

public class UsbRouter : EssentialsDevice, IRoutingWithFeedback
{
    public UsbRouter(string key) : base(key)
    {
        InputPorts = new RoutingPortCollection<RoutingInputPort>();
        OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

        AddPostActivationAction(AddRoutingPorts);
        AddPostActivationAction(AddFeedbackMatchObjects);
    }

    public event RouteChangedEventHandler RouteChanged;


    #region IRouting Members    

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        if (!signalType.HasFlag(eRoutingSignalType.UsbInput) &&
            !signalType.HasFlag(eRoutingSignalType.UsbOutput))
        {
            this.LogError("Invalid signal type for USB routing: {0}", signalType);
            return;
        }

        var localDevice = inputSelector as IUsbStreamWithHardware;

        var remoteDevice = outputSelector as IUsbStreamWithHardware;

        // clear the route if the local device is null
        if (localDevice == null && remoteDevice != null)
        {
            remoteDevice.ClearCurrentUsbRoute();
            return;
        }

        //nothing to route if both selectors are null
        if (localDevice == null && remoteDevice == null)
        {
            this.LogError("Both input and output selectors are null. No routing action taken.");
            return;
        }

        this.LogDebug("Pairing {0} to {1}", localDevice.UsbLocalId, remoteDevice.UsbLocalId);

        // Choosing to make a route from the remote device to the local device.
        // The logic in the `MakeUsbRoute` method will handle the actual routing, along with clearing any previous routes.
        remoteDevice.MakeUsbRoute(localDevice);

        UpdateCurrentRoutes(localDevice, remoteDevice);
    }

    public void TestUsbRoute(string inputPortKey, string outputPortKey)
    {
        var inputPort = InputPorts[inputPortKey];

        var outputPort = OutputPorts[outputPortKey];

        if (outputPort == null)
        {
            this.LogError("Unable to find device port for {outputPortKey}", outputPortKey);
            return;
        }

        ExecuteSwitch(inputPort?.Selector ?? null, outputPort.Selector, eRoutingSignalType.UsbInput);
    }

    private void UpdateCurrentRoutes(IUsbStreamWithHardware local, IUsbStreamWithHardware remote)
    {
        this.LogDebug("Updating current routes for USB Router: {local} -> {remote}", local?.Key ?? "null", remote?.Key ?? "null");
        RouteSwitchDescriptor descriptor;

        if (local == null && remote == null)
        {
            this.LogDebug("Both local and remote devices are null. Clearing all current routes.");
            CurrentRoutes.Clear();
            RouteChanged?.Invoke(this, null);
            return;
        }

        if (local != null && remote == null)
        {
            this.LogDebug("Remote device is null. Clearing route for local device {local}", local.Key);
            descriptor = GetRouteDescriptorByInputPort(local);

            if (descriptor != null)
            {
                CurrentRoutes.Remove(descriptor);
                RouteChanged?.Invoke(this, descriptor);
            }
            return;
        }

        descriptor = GetRouteDescriptorByOutputPort(remote);

        this.LogDebug("Found existing route descriptor: [{descriptor}]", descriptor?.ToString() ?? "null");

        var inputPort = GetRoutingInputPortForSelector(local);

        var outputPort = GetRoutingOutputPortForSelector(remote);

        if (outputPort is null)
        {
            this.LogWarning("Unable to find port for {rx}", this, remote.Key);
            return;
        }

        if (descriptor is null && outputPort is not null)
        {
            descriptor = new(outputPort, inputPort);

            CurrentRoutes.Add(descriptor);
        }
        else
        {
            descriptor.InputPort = inputPort;
        }

        this.LogDebug("Updated route descriptor: [{descriptor}]", descriptor?.ToString());

        RouteChanged?.Invoke(this, descriptor);
    }

    private RouteSwitchDescriptor GetRouteDescriptorByOutputPort(IUsbStreamWithHardware rx)
    {
        return CurrentRoutes.FirstOrDefault(rd =>
        {
            if (rd.OutputPort.Selector is not IUsbStreamWithHardware selector)
            {
                return false;
            }

            return selector.Key == rx.Key;
        });
    }

    private RouteSwitchDescriptor GetRouteDescriptorByInputPort(IUsbStreamWithHardware tx)
    {
        return CurrentRoutes.FirstOrDefault(rd =>
        {
            if (rd.InputPort.Selector is not IUsbStreamWithHardware selector)
            {
                return false;
            }

            return selector.Key == tx.Key;
        });
    }

    private RoutingInputPort GetRoutingInputPortForSelector(IUsbStreamWithHardware tx)
    {
        if (tx == null) return null;

        return InputPorts.FirstOrDefault(ip =>
        {
            if (ip.Selector is not IUsbStreamWithHardware selector)
            {
                return false;
            }

            return selector.Key == tx.Key;
        });
    }

    private RoutingOutputPort GetRoutingOutputPortForSelector(IUsbStreamWithHardware rx)
    {
        if (rx == null) return null;

        return OutputPorts.FirstOrDefault(ip =>
        {
            if (ip.Selector is not IUsbStreamWithHardware selector)
            {
                return false;
            }

            return selector.Key == rx.Key;
        });

    }

    private void AddFeedbackMatchObjects()
    {
        // This method is used to add feedback match objects for the USB router.
        // It can be used to set up feedback for the input and output ports.
        // For now, we will just log that this method has been called.
        this.LogDebug("Adding feedback match objects for USB Router");

        var ports = InputPorts.Where(ip => ip.Selector is IUsbStreamWithHardware);

        this.LogDebug("{count} input ports found", ports.Count());

        foreach (var port in ports)
        {
            if (port.Selector is not IUsbStreamWithHardware device)
            {
                continue;
            }

            if (device.IsRemote)
            {
                this.LogInformation("Skipping remote device {deviceKey}", device.Key);
                continue;
            }

            // getting current local device for this remote when it changes and setting the feedback match object for this input port to that value

            device.Hardware.UsbInput.UsbInputChange += (o, a) =>
            {
                if (a.EventId != UsbInputEventIds.LocalDeviceIdFeedbackEventId)
                {
                    return;
                }

                this.LogDebug("Updating USB Feedback match object for {portKey} to {localId}", port.Key, device.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue);
                port.FeedbackMatchObject = device.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;
            };

            this.LogDebug("Updating USB Feedback match object for {portKey} to {localId}", port.Key, device.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue);
            port.FeedbackMatchObject = device.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;
        }
    }

    private void AddRoutingPorts()
    {
        this.LogDebug("Adding routing ports for USB Router");
        // Remote devices in NVX world are the USB peripherals like keyboards or touchscreen
        var usbRemoteDevices = DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>().Where(usb => usb.IsRemote);

        // Local devices in NVX world are the USB Hosts like a PC
        var usbLocalDevices = DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>()
            .Where(usb => !usb.IsRemote);

        // A local device can have multiple remote devices, but a remote device can only have one local device.
        // remote devices will be treated as Outputs and local devices as Inputs to mimic traditional video routing behavior.

        foreach (var remoteDevice in usbRemoteDevices)
        {
            var outputPort = new RoutingOutputPort($"{remoteDevice.Key}-UsbRemote", eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput, eRoutingPortConnectionType.UsbC, remoteDevice, this);
            OutputPorts.Add(outputPort);

            remoteDevice.Hardware.UsbInput.UsbInputChange += (o, a) =>
            {
                if (a.EventId != UsbInputEventIds.PairedEventId && a.EventId != UsbInputEventIds.RemoteDeviceIdFeedbackEventId)
                {
                    return;
                }

                if (a.EventId == UsbInputEventIds.RemoteDeviceIdFeedbackEventId && a.Index > 1)
                {
                    // only care about index 1 which is the active remote device id
                    this.LogDebug("Ignoring RemoteDeviceIdFeedbackEventId with index {index} for {deviceKey}", a.Index, remoteDevice.Key);
                    return;
                }

                var paired = remoteDevice.Hardware.UsbInput.PairFeedback[1].BoolValue;

                // currently only one local device can be paired to a remote device
                this.LogDebug("USB Route change detected for {deviceKey} - {paired}", remoteDevice.Key, paired ? "paired" : "unpaired");

                var localDeviceId = remoteDevice.Hardware.UsbInput.RemoteDeviceIdFeedbacks[1].StringValue;

                this.LogDebug("Remote device {deviceKey} paired to local device ID: {localId}", remoteDevice.Key, localDeviceId);

                var localDevice = usbLocalDevices.FirstOrDefault(d => d.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue == localDeviceId);

                this.LogDebug("Found local device for pairing: {localDeviceKey}", localDevice?.Key ?? "null");

                var localForRoute = paired ? localDevice : null;

                UpdateCurrentRoutes(localForRoute, remoteDevice);
            };
        }

        foreach (var localDevice in usbLocalDevices)
        {
            var inputPort = new RoutingInputPort($"{localDevice.Key}-UsbLocal", eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput, eRoutingPortConnectionType.UsbC, localDevice, this);
            InputPorts.Add(inputPort);

            localDevice.Hardware.UsbInput.UsbInputChange += (o, a) =>
            {
                if (a.EventId != UsbInputEventIds.PairedEventId && a.EventId != UsbInputEventIds.RemoteDeviceIdFeedbackEventId)
                {
                    return;
                }

                if (a.EventId == UsbInputEventIds.RemoteDeviceIdFeedbackEventId && a.Index > 1)
                {
                    // only care about index 1 which is the active remote device id
                    this.LogDebug("Ignoring RemoteDeviceIdFeedbackEventId with index {index} for {deviceKey}", a.Index, localDevice.Key);
                    return;
                }

                var paired = localDevice.Hardware.UsbInput.PairFeedback[1].BoolValue;

                this.LogDebug("USB Route change detected for {deviceKey} - {paired}", localDevice.Key, paired ? "paired" : "unpaired");

                var remoteDeviceId = localDevice.Hardware.UsbInput.RemoteDeviceIdFeedbacks[1].StringValue;

                this.LogDebug("Local device {deviceKey} paired to remote device ID: {remoteId}", localDevice.Key, remoteDeviceId);

                // var remoteDevice = usbRemoteDevices.FirstOrDefault(d => d.UsbLocalId.StringValue == remoteDeviceId);

                // this.LogDebug("Found remote device for pairing: {remoteDeviceKey}", remoteDevice?.Key ?? "null");

                // var remoteForRoute = paired ? remoteDevice : null;
                // UpdateCurrentRoutes(localDevice, remoteForRoute);
            };
        }

        var clearRoutePort = new RoutingInputPort("None", eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput, eRoutingPortConnectionType.UsbC, null, this);
        InputPorts.Add(clearRoutePort);
    }

    #endregion

    #region IRoutingInputs Members

    public RoutingPortCollection<RoutingInputPort> InputPorts
    {
        get; private set;
    }

    #endregion

    #region IRoutingOutputs Members

    public RoutingPortCollection<RoutingOutputPort> OutputPorts
    {
        get; private set;
    }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    #endregion
}