using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Devices;
using NvxEpi.Services.TieLines;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Features.Routing;

public class NvxGlobalRouter : EssentialsDevice, IRoutingMidpointWithFeedback, IHasNamedRoutingSlots
{
    private static readonly NvxGlobalRouter _instance = new();

    public const string InstanceKey = "NvxRouter";
    public const string RouteOff = "$off";
    public const string NoSourceText = "No Source";

    public IRoutingMidpointWithFeedback PrimaryStreamRouter { get; private set; }
    public IRoutingMidpointWithFeedback SecondaryAudioRouter { get; private set; }

    public IRoutingMidpointWithFeedback UsbRouter { get; private set; }

    public event RouteChangedEventHandler RouteChanged;

    private NvxGlobalRouter()
        : base(InstanceKey)
    {
        PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
        SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");
        UsbRouter = new UsbRouter(Key + "-Usb");

        // Forward each sub-router's RouteChanged through this device's own event -
        // preserves the real per-router tracking rather than inventing a parallel one.
        PrimaryStreamRouter.RouteChanged += (sender, route) => RouteChanged?.Invoke(this, route);
        SecondaryAudioRouter.RouteChanged += (sender, route) => RouteChanged?.Invoke(this, route);
        UsbRouter.RouteChanged += (sender, route) => RouteChanged?.Invoke(this, route);

        InputPorts = new RoutingPortCollection<RoutingInputPort>();
        OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

        DeviceManager.AddDevice(PrimaryStreamRouter);
        DeviceManager.AddDevice(SecondaryAudioRouter);
        DeviceManager.AddDevice(UsbRouter);

        AddPostActivationAction(BuildTieLines);

        AddPostActivationAction(BuildMatrixRouting);

        //InputSlots = new Dictionary<string, INvxInputSlot>();
        //OutputSlots = new Dictionary<string, INvxOutputSlot>();
    }

    public static NvxGlobalRouter Instance
    {
        get { return _instance; }
    }

    private static void BuildTieLines()
    {
        var transmitters = DeviceManager
            .AllDevices.OfType<INvxDevice>()
            .Where(t => t.IsTransmitter)
            .ToList();

        TieLineConnector.AddTieLinesForTransmitters(transmitters);

        var receivers = DeviceManager
            .AllDevices.OfType<INvxDevice>()
            .Where(t => !t.IsTransmitter)
            .ToList();

        TieLineConnector.AddTieLinesForReceivers(receivers);

        var audioTransmitters = DeviceManager
            .AllDevices.OfType<INvxDevice>()
            .Where(t => t.IsTransmitter)
            .ToList();

        TieLineConnector.AddTieLinesForAudioTransmitters(audioTransmitters);

        var audioReceivers = DeviceManager
            .AllDevices.OfType<INvxDevice>()
            .Where(t => !t.IsTransmitter)
            .ToList();

        TieLineConnector.AddTieLinesForAudioReceivers(audioReceivers);
    }

    public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
    public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

    public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
    {
        // Selector may be the port's real stream object or, from mobile control's matrix routing,
        // the named slot key (= slot Key) - resolve back to the transmitter/receiver device before
        // delegating, or the sub-routers' casts silently drop the switch.
        var resolvedInput = ResolveInputSelector(inputSelector);
        var resolvedOutput = ResolveOutputSelector(outputSelector);

        if (signalType.Has(eRoutingSignalType.Video))
            PrimaryStreamRouter.ExecuteSwitch(resolvedInput, resolvedOutput, signalType);

        if (
            signalType.Has(eRoutingSignalType.Audio)
            || signalType.Has(eRoutingSignalType.AudioVideo)
        )
            SecondaryAudioRouter.ExecuteSwitch(resolvedInput, resolvedOutput, signalType);

        if (signalType.HasFlag(eRoutingSignalType.Usb))
            UsbRouter.ExecuteSwitch(resolvedInput, resolvedOutput, signalType);
    }

    public void ClearRoute(object outputSelector, eRoutingSignalType signalType)
    {
        var resolvedOutput = ResolveOutputSelector(outputSelector);

        if (signalType.Has(eRoutingSignalType.Video))
            PrimaryStreamRouter.ClearRoute(resolvedOutput, signalType);

        if (
            signalType.Has(eRoutingSignalType.Audio)
            || signalType.Has(eRoutingSignalType.AudioVideo)
        )
            SecondaryAudioRouter.ClearRoute(resolvedOutput, signalType);

        if (signalType.HasFlag(eRoutingSignalType.Usb))
            UsbRouter.ClearRoute(resolvedOutput, signalType);
    }

    // Non-string selectors (the EISC bridge path, which already passes the real device) pass
    // through unchanged; an unmatched key also passes through so the sub-router reports/rejects
    // it the same way it always has. NvxMatrixClearInput's TxDeviceKey is empty, which
    // GetDeviceForKey resolves to null - the "clear this output" input the sub-routers expect.
    private object ResolveInputSelector(object selector)
    {
        if (selector is not string key)
            return selector;

        return InputSlots.TryGetValue(key, out var slot)
            ? DeviceManager.GetDeviceForKey(slot.TxDeviceKey)
            : selector;
    }

    private object ResolveOutputSelector(object selector)
    {
        if (selector is not string key)
            return selector;

        return OutputSlots.TryGetValue(key, out var slot)
            ? DeviceManager.GetDeviceForKey(slot.RxDeviceKey)
            : selector;
    }

    public List<RouteSwitchDescriptor> CurrentRoutes =>
        PrimaryStreamRouter.CurrentRoutes
            .Concat(SecondaryAudioRouter.CurrentRoutes)
            .Concat(UsbRouter.CurrentRoutes)
            .ToList();

    public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
    {
        throw new NotImplementedException("Execute Numeric Switch");
    }

    private Dictionary<string, INvxInputSlot> _inputSlots = new();
    private Dictionary<string, INvxOutputSlot> _outputSlots = new();
    public Dictionary<string, INvxInputSlot> InputSlots => _inputSlots.Where(kvp =>

            kvp.Value is NvxMatrixClearInput

            || kvp.Value is NvxMockMatrixInput

            || (kvp.Value is NvxMatrixInput input && input.IsEnabled))

        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    public Dictionary<string, INvxOutputSlot> OutputSlots => _outputSlots.Where(
        kvp => kvp.Value is NvxMatrixOutput output && output.IsEnabled)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value);

    // IHasNamedRoutingSlots view of the plugin-local slot dictionaries above. IReadOnlyDictionary
    // has no value-type variance, so the values are converted, not cast.
    IReadOnlyDictionary<string, IRoutingSlotInfo> IHasNamedRoutingSlots.InputSlots =>
        InputSlots.ToDictionary(kvp => kvp.Key, kvp => (IRoutingSlotInfo)kvp.Value);
    IReadOnlyDictionary<string, IRoutingOutputSlotInfo> IHasNamedRoutingSlots.OutputSlots =>
        OutputSlots.ToDictionary(kvp => kvp.Key, kvp => (IRoutingOutputSlotInfo)kvp.Value);


    private void BuildMatrixRouting()
    {
        try
        {
            _inputSlots = DeviceManager
                .AllDevices.OfType<NvxBaseDevice>()
                .Where(t => t.IsTransmitter)
                .Select(t =>
                {
                    return new NvxMatrixInput(t);
                })
                .Cast<INvxInputSlot>()
                .ToDictionary(i => i.Key, i => i);

            var mockInputSlots = DeviceManager
                .AllDevices.OfType<NvxMockDevice>()
                .Where(md => md.IncludeInMatrixRouting && md.IsTransmitter)
                .Select(md =>
                {
                    return new NvxMockMatrixInput(md);
                })
                .Cast<INvxInputSlot>()
                .ToDictionary(i => i.Key, i => i);

            this.LogDebug("Mock Device inputs: {count}", mockInputSlots.Count);
            this.LogDebug("Real Device inputs: {count}", _inputSlots.Count);

            foreach (var kvp in mockInputSlots)
            {
                _inputSlots[kvp.Key] = kvp.Value;
            }

            this.LogDebug("Total input: {count}", _inputSlots.Count);

            var clearInput = new NvxMatrixClearInput();

            _inputSlots.Add(clearInput.Key, clearInput);

            _outputSlots = DeviceManager
                .AllDevices.OfType<NvxBaseDevice>()
                .Where(t => !t.IsTransmitter)
                .Select((t) => new NvxMatrixOutput(t))
                .Cast<INvxOutputSlot>()
                .ToDictionary(t => t.Key, t => t);
        }
        catch (Exception ex)
        {
            Debug.LogMessage(ex, "Exception building MatrixRouting: {message}", this, ex.Message);
        }
    }

    public void Route(string inputSlotKey, string outputSlotKey, eRoutingSignalType type)
    {
        if (!InputSlots.TryGetValue(inputSlotKey, out var inputSlot))
        {
            this.LogError("Unable to find input slot with key {0}", inputSlotKey);
            return;
        }

        if (!OutputSlots.TryGetValue(outputSlotKey, out var outputSlot))
        {
            this.LogError("Unable to find output slot with key {0}", outputSlotKey);
            return;
        }

        if (outputSlot is not NvxMatrixOutput output)
        {
            Debug.LogMessage(
                Serilog.Events.LogEventLevel.Error,
                "Output with key {key} is not NvxMatrixOutput",
                this,
                outputSlotKey
            );
            return;
        }

        var outputDevice = output.Device;

        if (outputDevice == null)
        {
            this.LogError("Unable to get device to route");
            return;
        }

        if (type.Has(eRoutingSignalType.Video))
        {
            // using namespace to qualify type as `Route` is a static method
            Routing.PrimaryStreamRouter.Route(inputSlot.SlotNumber, outputDevice);
        }

        if (
            (type.Has(eRoutingSignalType.AudioVideo) || type.Has(eRoutingSignalType.Audio))
            && outputDevice is ISecondaryAudioStreamWithHardware audioOutput
        )
        {
            // using namespace to qualify type as `Route` is a static method
            Routing.SecondaryAudioRouter.Route(inputSlot.SlotNumber, audioOutput);
        }
    }
}
