using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Extensions;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Routing;

public class SecondaryAudioRouter : EssentialsDevice, IRoutingWithFeedback
{
    public SecondaryAudioRouter(string key) : base(key)
    {
        InputPorts = new RoutingPortCollection<RoutingInputPort>();
        OutputPorts = new RoutingPortCollection<RoutingOutputPort>();
        AddPostActivationAction(AddFeedbackMatchObjects);
    }

    private void AddFeedbackMatchObjects()
    {
        foreach (var input in InputPorts.Where(ip => ip.Selector is IStreamWithHardware))
        {
            if (input.Selector is not IStreamWithHardware tx)
            {
                continue;
            }

            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating match object for {key}", this, input.Key);

            tx.Hardware.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += (o, a) =>
            {
                if (a.EventId != DMOutputEventIds.MulticastAddressEventId) return;

                Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating Feedback match object for {input}", this, input.Key);

                Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Updating Feedback match object for {input} to {url}", this, input.Key, tx.Hardware.Control.ServerUrlFeedback.StringValue);

                input.FeedbackMatchObject = tx.Hardware.DmNaxRouting.DmNaxTransmit.MulticastAddressFeedback.StringValue;
            };

            input.FeedbackMatchObject = tx.Hardware.DmNaxRouting.DmNaxTransmit.MulticastAddressFeedback.StringValue;
        }
    }

    public override bool CustomActivate()
    {
        _transmitters ??= GetTransmitterDictionary();

        _receivers ??= GetReceiverDictionary();

        new[] { _transmitters.Values, _receivers.Values }
            .SelectMany(x => x)
            .ToList()
            .ForEach(device =>
                {
                    var streamInputPort = device.OutputPorts[SwitcherForSecondaryAudioOutput.Key];
                    if (streamInputPort != null)
                    {
                        var input = new RoutingInputPort(
                            GetInputPortKeyForTx(device),
                            eRoutingSignalType.Audio,
                            eRoutingPortConnectionType.Streaming,
                            device,
                            this);

                        InputPorts.Add(input);
                    }

                    var streamOutputPort = device.InputPorts[DeviceInputEnum.SecondaryAudio.Name];
                    if (streamOutputPort == null)
                        return;

                    var output = new RoutingOutputPort(
                        GetOutputPortKeyForRx(device),
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        device,
                        this);

                    OutputPorts.Add(output);
                });

        foreach (var routingOutputPort in OutputPorts)
        {
            var port = routingOutputPort;
            const int delayTime = 250;

            var timer = new CTimer(o =>
            {
                if (port.InUseTracker.InUseFeedback.BoolValue)
                    return;

                ExecuteSwitch(null, port.Selector, eRoutingSignalType.Audio);
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
            case DMInputEventIds.DmNaxAudioSourceFeedbackEventId:
                {
                    var currentUrl = device.Hardware.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue;

                    Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Received server URL event {eventId}:{serverUrl}", this, args.EventId, currentUrl);

                    if (string.IsNullOrEmpty(currentUrl))
                    {
                        var index = CurrentRoutes.FindIndex((r) => r.OutputPort.ParentDevice.Key == device.Key);

                        if (index < 0)
                        {
                            return;
                        }

                        CurrentRoutes.RemoveAt(index);

                        RouteChanged?.Invoke(this, null);
                        break;
                    }

                    var inputPort = InputPorts.FirstOrDefault(ip => ip.FeedbackMatchObject.Equals(currentUrl));

                    if (inputPort == null)
                    {
                        Debug.LogMessage(Serilog.Events.LogEventLevel.Information, "No input port found for URL {currentUrl}", this, currentUrl);
                        break;
                    }

                    var outputPort = OutputPorts.FirstOrDefault(op => op.ParentDevice.Key == device.Key);

                    if (outputPort == null)
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

    public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
    public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        try
        {
            Debug.LogMessage(Serilog.Events.LogEventLevel.Debug, "Trying execute switch secondary audio route: {input} {output}", NvxGlobalRouter.Instance.SecondaryAudioRouter, inputSelector, outputSelector);

            if (!signalType.Has(eRoutingSignalType.Audio) && !signalType.Has(eRoutingSignalType.SecondaryAudio))
                throw new ArgumentException("signal type must include audio or secondary audio");

            var rx = outputSelector as ISecondaryAudioStream ?? throw new ArgumentNullException("rx");

            if (inputSelector is null)
            {
                rx.ClearSecondaryStream();
                return;
            }

            rx.RouteSecondaryAudio(inputSelector as ISecondaryAudioStream);
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Error executing route!", this);
            throw;
        }
    }

    public static string GetInputPortKeyForTx(ISecondaryAudioStream tx)
    {
        return tx.Key + "-SecondaryAudio";
    }

    public static string GetOutputPortKeyForRx(ISecondaryAudioStream rx)
    {
        return rx.Key + "-SecondaryAudioOutput";
    }

    public static void Route(int txId, int rxId)
    {
        Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying secondary audio route by txId & rxId: {0} {1}", txId, rxId);
        if (rxId == 0)
            return;

        var rx = _receivers.Values.FirstOrDefault(x => x.DeviceId == rxId);
        if (rx == null)
            return;

        if (txId == 0)
        {
            rx.ClearSecondaryStream();
            return;
        }

        Route(txId, rx);
    }

    public static void Route(int txId, ISecondaryAudioStream rx)
    {
        Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying secondary audio route by txId & address: {0} {1}", txId, rx.RxAudioAddress);
        if (txId == 0)
        {
            rx.ClearSecondaryStream();
            return;
        }

        var tx = _transmitters.Values.FirstOrDefault(x => x.DeviceId == txId);
        if (tx == null)
            return;

        rx.RouteSecondaryAudio(tx);
    }

    public static void Route(string txName, ISecondaryAudioStream rx)
    {
        Debug.Console(1, NvxGlobalRouter.Instance.SecondaryAudioRouter, "Trying secondary audio route by txName & address: {0} {1}", txName, rx.RxAudioAddress);
        if (string.IsNullOrEmpty(txName))
            return;

        if (txName.Equals(NvxGlobalRouter.RouteOff, StringComparison.OrdinalIgnoreCase))
        {
            rx.ClearSecondaryStream();
            return;
        }

        if (_transmitters.TryGetValue(txName, out ISecondaryAudioStream txByName))
        {
            rx.RouteSecondaryAudio(txByName);
            return;
        }

        var txByKey = _transmitters
            .Values
            .FirstOrDefault(x => x.Key.Equals(txName, StringComparison.OrdinalIgnoreCase));

        if (txByKey == null)
            return;

        rx.RouteSecondaryAudio(txByKey);
    }

    private static Dictionary<string, ISecondaryAudioStream> _transmitters;
    private static Dictionary<string, ISecondaryAudioStream> _receivers;

    public event RouteChangedEventHandler RouteChanged;

    private static Dictionary<string, ISecondaryAudioStream> GetTransmitterDictionary()
    {
        return
            DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .Where(device => device.IsTransmitter)
                .ToDictionary(device => device.Key, stream => stream);
    }

    private static Dictionary<string, ISecondaryAudioStream> GetReceiverDictionary()
    {
        return
            DeviceManager
                .AllDevices
                .OfType<ISecondaryAudioStream>()
                .Where(device => !device.IsTransmitter)
                .ToDictionary(device => device.Key, stream => stream);
    }
}