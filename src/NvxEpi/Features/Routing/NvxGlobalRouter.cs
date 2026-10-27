using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Devices;
using NvxEpi.Extensions;
using NvxEpi.Services.TieLines;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;

namespace NvxEpi.Features.Routing;

public class NvxGlobalRouter : EssentialsDevice, IRoutingNumeric, IMatrixRouting
{
    private static readonly NvxGlobalRouter _instance = new();

    public const string InstanceKey = "NvxRouter";
    public const string RouteOff = "$off";
    public const string NoSourceText = "No Source";

    public IRouting PrimaryStreamRouter { get; private set; }
    public IRouting SecondaryAudioRouter { get; private set; }

    public IRouting UsbRouter { get; private set; }

    private NvxGlobalRouter()
        : base(InstanceKey)
    {
        PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
        SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");
        UsbRouter = new UsbRouter(Key + "-Usb");

        InputPorts = new RoutingPortCollection<RoutingInputPort>();
        OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

        DeviceManager.AddDevice(PrimaryStreamRouter);
        DeviceManager.AddDevice(SecondaryAudioRouter);
        DeviceManager.AddDevice(UsbRouter);

        AddPostActivationAction(BuildTieLines);

        AddPostActivationAction(BuildMatrixRouting);

        //InputSlots = new Dictionary<string, IRoutingInputSlot>();
        //OutputSlots = new Dictionary<string, IRoutingOutputSlot>();
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
        if (signalType.Has(eRoutingSignalType.Video))
            PrimaryStreamRouter.ExecuteSwitch(inputSelector, outputSelector, signalType);

        if (
            signalType.Has(eRoutingSignalType.Audio)
            || signalType.Has(eRoutingSignalType.SecondaryAudio)
        )
            SecondaryAudioRouter.ExecuteSwitch(inputSelector, outputSelector, signalType);

        if (
            signalType.HasFlag(eRoutingSignalType.UsbInput)
            || signalType.HasFlag(eRoutingSignalType.UsbOutput)
        )
            UsbRouter.ExecuteSwitch(inputSelector, outputSelector, signalType);
    }

    public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
    {
        throw new NotImplementedException("Execute Numeric Switch");
    }

    private Dictionary<string, IRoutingInputSlot> _inputSlots = new();
    private Dictionary<string, IRoutingOutputSlot> _outputSlots = new();
public Dictionary<string, IRoutingInputSlot> InputSlots => _inputSlots.Where(kvp =>
        kvp.Value is NvxMatrixClearInput
        || kvp.Value is NvxMockMatrixInput
        || (kvp.Value is NvxMatrixInput input && input.IsEnabled))
    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    public Dictionary<string, IRoutingOutputSlot> OutputSlots => _outputSlots.Where(
        kvp => kvp.Value is NvxMatrixOutput output && output.IsEnabled)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value);


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
                .Cast<IRoutingInputSlot>()
                .ToDictionary(i => i.Key, i => i);

            var mockInputSlots = DeviceManager
                .AllDevices.OfType<NvxMockDevice>()
                .Where(md => md.IncludeInMatrixRouting && md.IsTransmitter)
                .Select(md =>
                {
                    return new NvxMockMatrixInput(md);
                })
                .Cast<IRoutingInputSlot>()
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
                .Cast<IRoutingOutputSlot>()
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

        var routeVideo = type.Has(eRoutingSignalType.Video);
        var routeAudio =
            type.Has(eRoutingSignalType.SecondaryAudio) || type.Has(eRoutingSignalType.Audio);

        // capture the current sources before the video route changes them
        var currentVideoId = outputDevice is ICurrentStream currentStream
            ? currentStream.CurrentStreamId.IntValue
            : 0;
        var currentAudioId = GetCurrentStreamAudioId(outputDevice, currentVideoId);

        if (routeVideo)
        {
            // using namespace to qualify type as `Route` is a static method
            Routing.PrimaryStreamRouter.Route(inputSlot.SlotNumber, outputDevice);
        }

        if (outputDevice is not ISecondaryAudioStreamWithHardware audioOutput)
            return;

        var videoId = routeVideo ? inputSlot.SlotNumber : currentVideoId;

        int audioId;

        if (routeAudio)
        {
            audioId = inputSlot.SlotNumber;
        }
        else if (currentAudioId.HasValue && currentAudioId.Value != 0)
        {
            // video-only route: keep the audio on its current source, moving it between the
            // primary and secondary paths as needed
            audioId = currentAudioId.Value;
        }
        else
        {
            return;
        }

        RouteAudio(audioId, videoId, audioOutput);
    }

    /// <summary>
    /// Uses the primary stream audio when the audio source matches the video source,
    /// otherwise routes the audio over the secondary audio (NAX) stream
    /// </summary>
    private void RouteAudio(int audioId, int videoId, ISecondaryAudioStreamWithHardware audioOutput)
    {
        if (audioId != 0 && audioId == videoId && audioOutput is ICurrentAudioInput audioInput)
        {
            this.LogDebug(
                "Audio source {audioId} matches video source on {device}, using primary stream audio",
                audioId,
                audioOutput.Key
            );

            audioInput.SetAudioToPrimaryStreamAudio();
            audioOutput.ClearSecondaryStream();
            return;
        }

        // using namespace to qualify type as `Route` is a static method
        Routing.SecondaryAudioRouter.Route(audioId, audioOutput);
    }

    /// <summary>
    /// Gets the id of the transmitter currently providing streamed audio to the device,
    /// or null if the device is using a local audio input
    /// </summary>
    private static int? GetCurrentStreamAudioId(IStreamWithHardware device, int currentVideoId)
    {
        var audioSource = device.Hardware.Control.AudioSourceFeedback;

        if (audioSource == DmNvxControl.eAudioSource.PrimaryStreamAudio)
            return currentVideoId;

        if (
            audioSource == DmNvxControl.eAudioSource.DmNaxAudio
            && device is ICurrentSecondaryAudioStream secondaryAudio
        )
            return secondaryAudio.CurrentSecondaryAudioStreamId.IntValue;

        return null;
    }
}
