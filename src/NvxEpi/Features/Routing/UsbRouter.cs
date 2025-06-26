using System.Collections.Generic;
using System.Linq;
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
        if (localDevice == null && remoteDevice != null ) { 
            remoteDevice.ClearCurrentUsbRoute();
            return;
        }

        //nothing to route if both selectors are null
        if(localDevice == null && remoteDevice == null)
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

    private void UpdateCurrentRoutes(IUsbStreamWithHardware local, IUsbStreamWithHardware remote)
    {
        RouteSwitchDescriptor descriptor;

        descriptor = GetRouteDescriptorByOutputPort(remote);

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

        var ports = InputPorts.OfType<RoutingInputPort>();

        foreach (var port in ports) {
            if(port.Selector is not IUsbStreamWithHardware device)
            {
                this.LogError("Input port {portKey} does not have a valid USB device selector.", port.Key);
                continue;
            }

            if (!device.IsRemote)
            {
                this.LogInformation("Skipping remote device {deviceKey}", device.Key);
                continue;
            }

            // getting current local device for this remote when it changes and setting the feedback match object for this input port to that value

            device.Hardware.UsbInput.UsbInputChange += (o, a) =>
            {
                if (a.EventId != UsbInputEventIds.PairedEventId)
                {
                    return;
                }

                port.FeedbackMatchObject = device.Hardware.UsbInput.RemoteDeviceIdFeedback.StringValue;
            };

            port.FeedbackMatchObject = device.Hardware.UsbInput.RemoteDeviceIdFeedback.StringValue;
        }
    }

    public override bool CustomActivate()
    {

        // Local devices in NVX world are the USB peripherals like keyboards or touchscreen
        var usbRemoteDevices = DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>().Where(usb => usb.IsRemote);

        // Remote devices in NVX world are the USB Hosts like a PC 
        var usbLocalDevices = DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>()
            .Where(usb => !usb.IsRemote);

        // A local device can have multiple remote devices, but a remote device can only have one local device.
        // remote devices will be treated as Outputs and local devices as Inputs to mimic traditional video routing behavior.

        foreach (var remoteDevice in usbRemoteDevices)
        {
            var outputPort = new RoutingOutputPort($"{remoteDevice.Key}-UsbRemote", eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput, eRoutingPortConnectionType.UsbC, remoteDevice, this);
            OutputPorts.Add(outputPort);
        }

        foreach (var localDevice in usbLocalDevices)
        {
            var inputPort = new RoutingInputPort($"{localDevice.Key}-UsbLocal", eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput, eRoutingPortConnectionType.UsbC, localDevice, this);
            InputPorts.Add(inputPort);
        }

        return base.CustomActivate();
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