using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing;

public class PrimaryStreamRouter : EssentialsDevice, IRoutingWithFeedback
{
    public PrimaryStreamRouter(string key) : base(key)
    {
        InputPorts = new RoutingPortCollection<RoutingInputPort>();
        OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

        AddPostActivationAction(AddFeedbackMatchObjects);
    }

    private void AddFeedbackMatchObjects()
    {
        Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating feedback match objects", this);

        foreach(var input in InputPorts.Where(ip => ip.Selector is IStreamWithHardware))
        {
            if(input.Selector is not IStreamWithHardware tx)
            {
                continue;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating match object for {key}",this, input.Key);

            tx.Hardware.BaseEvent += (o, a) => {               
                if (a.EventId != DMInputEventIds.ServerUrlEventId) return;

                Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating Feedback match object for {input}", this, input.Key);

                Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating Feedback match object for {input} to {url}", this, input.Key, tx.Hardware.Control.ServerUrlFeedback.StringValue);

                input.FeedbackMatchObject = tx.Hardware.Control.ServerUrlFeedback.StringValue;
            };

            input.FeedbackMatchObject = tx.Hardware.Control.ServerUrlFeedback.StringValue;
        }
    }

    public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
    public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            if (signalType.Is(eRoutingSignalType.Audio))
            {
                Debug.LogMessage(
                    Serilog.Events.LogEventLevel.Information,
                    "Executing switch, but its audio only... this route will include video... GOOD LUCK!",
                    this);
            }

            var rx = outputSelector as IStreamWithHardware ?? throw new ArgumentNullException("rx");

            if (inputSelector is not IStream tx || inputSelector is null)
            {
                rx.ClearStream();

                UpdateCurrentRoutes(null, rx);

                return;
            }

            rx.RouteStream(tx);

            UpdateCurrentRoutes(tx, rx);

            if (!signalType.Has(eRoutingSignalType.Audio)) 
                return;

            if (rx is not ICurrentAudioInput audioInputSwitcher)
                return;

            audioInputSwitcher.SetAudioToPrimaryStreamAudio();
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Error executing route!", this);
        }
    }

    private void UpdateCurrentRoutes(IStream tx, IStream rx)
    {        
        RouteSwitchDescriptor descriptor;

        descriptor = GetRouteDescriptorByOutputPort(rx);

        var inputPort = GetRoutingInputPortForSelector(tx);

        var outputPort = GetRoutingOutputPortForSelector(rx);

        if(outputPort is null)
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Warning, "Unable to find port for {rx}", this, rx.Key);
            return;
        }
        
        if(descriptor is null && outputPort is not null)
        {
            descriptor = new(outputPort, inputPort);

            CurrentRoutes.Add(descriptor);
        } else
        {
            descriptor.InputPort = inputPort;
        }

        RouteChanged?.Invoke(this, descriptor);
    }

    private RouteSwitchDescriptor GetRouteDescriptorByOutputPort(IStream rx) {
        return CurrentRoutes.FirstOrDefault(rd =>
        {
            if (rd.OutputPort.Selector is not IStream selector)
            {
                return false;
            }

            return selector.Key == rx.Key;
        });
    }

    private RoutingInputPort GetRoutingInputPortForSelector(IStream tx) { 
        if (tx == null) return null;

        return InputPorts.FirstOrDefault(ip =>
        {
            if (ip.Selector is not IStream selector)
            {
                return false;
            }

            return selector.Key == tx.Key;
        });
    }

    private RoutingOutputPort GetRoutingOutputPortForSelector(IStream rx) {
        if (rx == null) return null;

        return OutputPorts.FirstOrDefault(ip =>
        {
            if (ip.Selector is not IStream selector)
            {
                return false;
            }

            return selector.Key == rx.Key;
        });

    }

    public override bool CustomActivate()
    {
        _transmitters ??= GetTransmitterDictionary();

        _receivers ??= GetReceiverDictionary();

        _transmitters
            .Values
            .ToList()
            .ForEach(tx =>
                {
                    var streamRoutingPort = tx.OutputPorts[SwitcherForStreamOutput.Key];
                    if (streamRoutingPort == null)
                        return;

                    var input = new RoutingInputPort(
                        GetInputPortKeyForTx(tx),
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        tx,
                        this);

                    InputPorts.Add(input);
                });

        _receivers
            .Values
            .ToList()
            .ForEach(rx =>
                {
                    var streamRoutingPort = rx.InputPorts[DeviceInputEnum.Stream.Name];
                    if (streamRoutingPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(rx),
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        rx,
                        this);                    

                    OutputPorts.Add(output);

                    if(rx is IStreamWithHardware rxHardware)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Subscribing to base event for {key}", this, rxHardware.Key);
                        rxHardware.Hardware.BaseEvent += (o, a) => HandleRouteUpdate(rxHardware, a);
                    }
                });

        foreach (var routingOutputPort in OutputPorts)
        {
            var port = routingOutputPort;
            const int delayTime = 250;

            var timer = new CTimer(o =>
                {
                    if (port.InUseTracker.InUseFeedback.BoolValue)
                        return;

                    ExecuteSwitch(null, port.Selector, eRoutingSignalType.AudioVideo);
                }, Timeout.Infinite);

            port.InUseTracker.InUseFeedback.OutputChange += (sender, args) =>
            {
                if (args.BoolValue)
                    return;

                timer.Reset(delayTime);
            };
        }

        return base.CustomActivate();
    }

    private void HandleRouteUpdate(IStreamWithHardware device, BaseEventArgs args)
    {       
        switch (args.EventId)
        {
            case DMInputEventIds.ServerUrlEventId:
                {
                    if(device == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "Device is null", this);
                        return;
                    }                    

                    var currentUrl = device.Hardware.Control.ServerUrlFeedback.StringValue;

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Received Server URL event {deviceKey};{eventId}:{serverUrl}", this, device?.Key, args.EventId, currentUrl);

                    var existingRoute = CurrentRoutes.FirstOrDefault((cr) => (cr.OutputPort.Selector as IKeyed)?.Key == device.Key);

                    if (string.IsNullOrEmpty(currentUrl) && existingRoute != null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Removing route {currentRoute}", this, existingRoute);

                        CurrentRoutes.Remove(existingRoute);                        

                        RouteChanged?.Invoke(this, null);
                        break;                        
                    }

                    var inputPort = InputPorts.FirstOrDefault(ip => {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Checking {currentUrl} against {feedbackMatchObject}", this, currentUrl, ip.FeedbackMatchObject);
                        return ip.FeedbackMatchObject != null && ip.FeedbackMatchObject.Equals(currentUrl); });

                    if(inputPort == null && existingRoute != null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No input port found for URL {currentUrl}",this, currentUrl);

                        Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Removing route {currentRoute}", this, existingRoute);

                        CurrentRoutes.Remove(existingRoute);

                        RouteChanged?.Invoke(this, null);

                        break;
                    }

                    if(existingRoute != null)
                    {                        
                        existingRoute.InputPort = inputPort;

                        RouteChanged?.Invoke(this, existingRoute);
                        return;
                    }

                    if(inputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No input port found for URL {currentUrl}", this, currentUrl);
                        return;
                    }

                    var outputPort = OutputPorts.FirstOrDefault(op => (op.Selector as IKeyed)?.Key == device.Key);
                    
                    if(outputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No output port found for {deviceKey}", this, device.Key);
                        break;
                    }

                    var route = new RouteSwitchDescriptor(outputPort, inputPort);

                    CurrentRoutes.Add(route);

                    RouteChanged?.Invoke(this, route);

                    break;
                }
        }
    }

    public static string GetInputPortKeyForTx(IStream tx)
    {
        return tx.Key + "-Stream";
    }

    public static string GetOutputPortKeyForRx(IStream rx)
    {
        return rx.Key + "-StreamOutput";
    }

    public static void Route(int txId, int rxId)
    {
        if (rxId == 0)
            return;

        var rx = GetRxById(rxId);
        if (rx == null)
            return;

        Route(txId, rx);
    }

    public static IStreamWithHardware GetRxById(int rxId)
    {
        return _receivers.Values.OfType<IStreamWithHardware>().FirstOrDefault(x => x.DeviceId == rxId);
    }

    public static IStream GetTxById(int txId)
    {
        return _transmitters.Values.FirstOrDefault(x => x.DeviceId == txId);
    }

    public static void Route(int txId, IStreamWithHardware rx)
    {
        if (rx.IsTransmitter)
            throw new ArgumentException("rx device is transmitter");

        if (txId == 0)
        {
            rx.ClearStream();
            return;
        }

        var tx = GetTxById(txId);
        if (tx == null)
            return;

        rx.RouteStream(tx);
    }

    public static void Route(string txName, IStreamWithHardware rx)
    {
        if (rx.IsTransmitter)
            throw new ArgumentException("rx device is transmitter");

        if (string.IsNullOrEmpty(txName))
            return;

        if (txName.Equals(NvxGlobalRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
        {
            rx.ClearStream();
            return;
        }

        if (_transmitters.TryGetValue(txName, out IStream txByName))
        {
            rx.RouteStream(txByName);
            return;
        }

        var txByKey = _transmitters
            .Values
            .FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));

        if (txByKey == null)
            return;

        rx.RouteStream(txByKey);
    }

    private static Dictionary<string, IStream> _receivers;
    private static Dictionary<string, IStream> _transmitters;

    public event RouteChangedEventHandler RouteChanged;

    private static Dictionary<string, IStream> GetTransmitterDictionary()
    {
        return
            DeviceManager
                .AllDevices
                .OfType<IStream>()
                .Where(device => device.IsTransmitter)
                .ToDictionary(device => device.Name, stream => stream);
    }

    private static Dictionary<string, IStream> GetReceiverDictionary()
    {
        return
            DeviceManager
                .AllDevices
                .OfType<IStream>()
                .Where(device => !device.IsTransmitter)
                .ToDictionary(device => device.Name, stream => stream);
    }
}
