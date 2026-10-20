using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Features.Config;
using NvxEpi.JoinMaps;
using NvxEpi.McMessengers;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Devices;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Devices;

public class NvxMockDevice
    : ReconfigurableDevice,
        IMockStreamReceiver,
        ISecondaryAudioStream,
        IRoutingMidpointWithFeedback,
        IBridgeAdvanced,
        IHasFeedback,
        ICommunicationMonitor
{
    private object propertiesLock = new();
    private NvxMockDeviceProperties properties;

    public bool IncludeInMatrixRouting => properties?.IncludeInMatrixRouting ?? false;

    private readonly RoutingPortCollection<RoutingInputPort> _inputPorts = new();

    private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts = new();

    private string streamUrl =>
        properties != null && !string.IsNullOrEmpty(properties.StreamUrl)
            ? properties.StreamUrl
            : string.Empty;

    public NvxMockDevice(DeviceConfig dc, bool isTransmitter)
        : base(dc)
    {
        properties = dc.Properties.ToObject<NvxMockDeviceProperties>();
        if (properties == null)
        {
            this.LogError("************ PROPS IS NULL ************");
            throw new NullReferenceException("properties");
        }

        Feedbacks = new FeedbackCollection<Feedback>();

        DeviceId = properties.DeviceId;
        IsTransmitter = isTransmitter;
        sync = isTransmitter && (properties.SyncDetected ?? true);
        communicationMonitor = new MockOnlineMonitor(this);
        BuildFeedbacks();
        BuildRoutingPorts();
        BuildCurrentRoutes();
    }

    protected override void CustomSetConfig(DeviceConfig config)
    {
        lock (propertiesLock)
        {
            var newProperties = config.Properties.ToObject<NvxMockDeviceProperties>();

            if (newProperties == null)
            {
                this.LogError("************ PROPS IS NULL ************");
                throw new NullReferenceException("properties");
            }

            this.LogDebug(
                "Updating config with {@newConfig}\r\nold {@oldConfig}",
                newProperties,
                properties
            );

            properties = newProperties;

            foreach (var feedback in Feedbacks)
            {
                feedback.FireUpdate();
            }
        }
    }

    private void BuildFeedbacks()
    {
        IsOnline = new BoolFeedback("IsOnline", () => Online);
        DeviceMode = new IntFeedback("DeviceMode", () => 0);
        StreamUrl = new StringFeedback("StreamUrl", () => streamUrl);

        MulticastAddress = new StringFeedback(
            "MulticastVideoAddress",
            () =>
                !string.IsNullOrEmpty(properties.MulticastVideoAddress)
                    ? properties.MulticastVideoAddress
                    : string.Empty
        );

        IsStreamingVideo = new BoolFeedback(
            "isStreamingVideo",
            () => !string.IsNullOrEmpty(properties.StreamUrl)
        );

        VideoStreamStatus = new StringFeedback(
            "videoStreamStatus",
            () => !string.IsNullOrEmpty(properties.StreamUrl) ? "Streaming" : string.Empty
        );

        SecondaryAudioAddress = new StringFeedback(
            "secondaryAudioAddress",
            () =>
                !string.IsNullOrEmpty(properties.MulticastAudioAddress)
                    ? properties.MulticastAudioAddress
                    : string.Empty
        );

        TxAudioAddress = new StringFeedback(
            "txAudioAddress",
            () =>
                !string.IsNullOrEmpty(properties.MulticastAudioAddress)
                    ? properties.MulticastAudioAddress
                    : string.Empty
        );

        RxAudioAddress = new StringFeedback("rxAudioAddress", () => string.Empty);

        IsStreamingSecondaryAudio = new BoolFeedback(
            "isStreamingSecondaryAudio",
            () => !string.IsNullOrEmpty(properties.MulticastAudioAddress)
        );

        SecondaryAudioStreamStatus = new StringFeedback(
            "secondaryAudioStreamStatus",
            () =>
                !string.IsNullOrEmpty(properties.MulticastAudioAddress) ? "Streaming" : string.Empty
        );

        SyncDetected = new BoolFeedback("syncDetected", () => Sync);

        Feedbacks.AddRange(
            new Feedback[]
            {
                DeviceNameFeedback.GetFeedback(Name),
                MulticastAddress,
                IsStreamingVideo,
                VideoStreamStatus,
                SecondaryAudioAddress,
                TxAudioAddress,
                RxAudioAddress,
                IsStreamingSecondaryAudio,
                SecondaryAudioStreamStatus,
                IsOnline,
                StreamUrl,
                SyncDetected,
            }
        );

        IsOnline.FireUpdate();
    }

    private void BuildRoutingPorts()
    {
        InputPorts.Add(
            new RoutingInputPort(
                DeviceInputEnum.Hdmi1.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                DeviceInputEnum.Hdmi1,
                this
            )
        );

        InputPorts.Add(
            new RoutingInputPort(
                DeviceInputEnum.SecondaryAudio.Name,
                eRoutingSignalType.Audio | eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.SecondaryAudio,
                this
            )
        );

        OutputPorts.Add(
            new RoutingOutputPort(
                SwitcherForSecondaryAudioOutput.Key,
                eRoutingSignalType.Audio | eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.LineAudio,
                null,
                this
            )
        );

        OutputPorts.Add(
            new RoutingOutputPort(
                SwitcherForHdmiOutput.Key,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                null,
                this
            )
        );

        if (IsTransmitter)
        {
            OutputPorts.Add(
                new RoutingOutputPort(
                    SwitcherForStreamOutput.Key,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Streaming,
                    null,
                    this
                )
            );
        }
        else
        {
            InputPorts.Add(
                new RoutingInputPort(
                    DeviceInputEnum.Stream.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Streaming,
                    DeviceInputEnum.Stream,
                    this
                )
                {
                    FeedbackMatchObject = eSfpVideoSourceTypes.Stream,
                }
            );
        }
    }

    protected override bool CustomActivate()
    {
        Feedbacks.ToList().ForEach(x => x.FireUpdate());

        return base.CustomActivate();
    }

    protected override void CreateMobileControlMessengers()
    {
        var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();

        if (mc == null)
        {
            this.LogInformation("Mobile Control not found");
            return;
        }

        var messenger = new MockDeviceMessenger($"{Key}-mockDevice", $"/device/{Key}", this);

        mc.AddDeviceMessenger(messenger);

        base.CreateMobileControlMessengers();
    }

    public RoutingPortCollection<RoutingInputPort> InputPorts
    {
        get { return _inputPorts; }
    }

    public RoutingPortCollection<RoutingOutputPort> OutputPorts
    {
        get { return _outputPorts; }
    }

    public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
    {
        // The mock's crosspoints are fixed (a transmitter streams its HDMI input, a receiver shows its stream),
        // so a switch through the mock changes nothing; the stream a receiver shows comes from ReceiveStream.
        this.LogVerbose("Executing switch : {0}", signalType);
    }

    public void ClearRoute(object outputSelector, eRoutingSignalType signalType) { }

    public List<RouteSwitchDescriptor> CurrentRoutes { get; } = new();

    public event RouteChangedEventHandler RouteChanged;

    // The receiver's Stream -> HDMI crosspoint; its input is set while a stream is routed to it.
    private RouteSwitchDescriptor receiverRoute;

    /// <summary>
    /// The crosspoints a real NVX reports, so the routing feedback can trace a sink back through the mock:
    /// a transmitter always streams its HDMI input; a receiver shows its stream on HDMI once one is routed.
    /// </summary>
    private void BuildCurrentRoutes()
    {
        if (IsTransmitter)
        {
            var hdmiIn = InputPorts[DeviceInputEnum.Hdmi1.Name];
            var streamOut = OutputPorts[SwitcherForStreamOutput.Key];
            if (hdmiIn != null && streamOut != null)
                CurrentRoutes.Add(new RouteSwitchDescriptor(streamOut, hdmiIn));
            return;
        }

        var hdmiOut = OutputPorts[SwitcherForHdmiOutput.Key];
        if (hdmiOut == null)
            return;

        receiverRoute = new RouteSwitchDescriptor(hdmiOut, null);
        CurrentRoutes.Add(receiverRoute);
    }

    /// <summary>
    /// Called by the primary stream router in place of switching hardware. Takes on the transmitter's stream
    /// URL and video sync (no sync when cleared), and reports the Stream -> HDMI crosspoint.
    /// </summary>
    public void ReceiveStream(IStream tx)
    {
        if (IsTransmitter)
            return;

        var url = tx?.StreamUrl?.StringValue ?? string.Empty;
        this.LogInformation("MOCK receive stream from {tx} ({url})", tx?.Key ?? "none", url);

        SetStreamUrl(url);
        Sync = tx != null && (tx is not NvxMockDevice mockTx || mockTx.Sync);

        if (receiverRoute == null)
            return;

        receiverRoute.InputPort = tx == null ? null : InputPorts[DeviceInputEnum.Stream.Name];
        RouteChanged?.Invoke(this, receiverRoute);
    }

    public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
    {
        this.LogVerbose("Executing switch : {0}", type);
    }

    public FeedbackCollection<Feedback> Feedbacks { get; private set; }

    private bool isOnline = true;

    public bool Online
    {
        get => isOnline;
        private set
        {
            if (isOnline == value)
                return;

            isOnline = value;

            IsOnline.FireUpdate();
            communicationMonitor.SetOnline(value);
        }
    }

    // ---- ICommunicationMonitor: as a real NVX has (NvxBaseDevice), so status pages treat the mock alike ----

    private readonly MockOnlineMonitor communicationMonitor;

    public StatusMonitorBase CommunicationMonitor => communicationMonitor;

    /// <summary>
    /// Bench control of the communication status, from the console:
    /// <c>devjson {"deviceKey":"nvx-tx-atv","methodName":"SetCommStatus","params":["warning"]}</c>.
    /// Takes ok / warning / error / unknown (or the MonitorStatus names). Online follows: only "ok" is online.
    /// </summary>
    public void SetCommStatus(string status)
    {
        if (!MockOnlineMonitor.TryParse(status, out var parsed))
        {
            this.LogWarning("SetCommStatus: '{status}' is not ok, warning, error or unknown", status);
            return;
        }

        this.LogInformation("MOCK comm status {status}", parsed);
        communicationMonitor.SetStatus(parsed);
        if (isOnline != (parsed == MonitorStatus.IsOk))
        {
            isOnline = parsed == MonitorStatus.IsOk;
            IsOnline.FireUpdate();
        }
    }

    /// <summary>Reports OK while the mock is online and in error while it is set offline; nothing to poll. Set
    /// from the console with SetIsOnline / SetCommStatus (devjson).</summary>
    private sealed class MockOnlineMonitor : StatusMonitorBase
    {
        public MockOnlineMonitor(IKeyed parent)
            : base(parent, 10000, 30000)
        {
            Status = MonitorStatus.IsOk;
        }

        public override void Start() { }

        public override void Stop() { }

        public void SetOnline(bool online) => Status = online ? MonitorStatus.IsOk : MonitorStatus.InError;

        public void SetStatus(MonitorStatus status) => Status = status;

        public static bool TryParse(string text, out MonitorStatus status)
        {
            switch ((text ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "ok": case "isok": case "online": status = MonitorStatus.IsOk; return true;
                case "warning": case "inwarning": status = MonitorStatus.InWarning; return true;
                case "error": case "inerror": case "offline": status = MonitorStatus.InError; return true;
                case "unknown": case "statusunknown": status = MonitorStatus.StatusUnknown; return true;
                default: status = MonitorStatus.StatusUnknown; return false;
            }
        }
    }

    public BoolFeedback IsOnline { get; private set; }
    public IntFeedback DeviceMode { get; private set; }
    public bool IsTransmitter { get; private set; }
    public int DeviceId { get; private set; }
    public StringFeedback StreamUrl { get; private set; }
    public StringFeedback SecondaryAudioAddress { get; private set; }
    public StringFeedback TxAudioAddress { get; private set; }
    public StringFeedback RxAudioAddress { get; private set; }
    public BoolFeedback IsStreamingVideo { get; private set; }
    public StringFeedback VideoStreamStatus { get; private set; }
    public BoolFeedback IsStreamingSecondaryAudio { get; private set; }
    public StringFeedback SecondaryAudioStreamStatus { get; private set; }
    public StringFeedback MulticastAddress { get; private set; }

    private bool sync;

    public bool Sync
    {
        get { return sync; }
        set
        {
            if (sync == value)
                return;

            sync = value;
            SyncDetected.FireUpdate();
        }
    }

    public BoolFeedback SyncDetected { get; private set; }

    private void SetStreamUrl(string url)
    {
        if (url.Equals(properties.StreamUrl ?? string.Empty))
            return;

        properties.StreamUrl = url;
        StreamUrl.FireUpdate();
        IsStreamingVideo.FireUpdate();
        VideoStreamStatus.FireUpdate();
    }

    public void SetSyncState(bool state)
    {
        Sync = state;
    }

    /// <summary>
    /// Bench control, from the console:
    /// <c>devjson {"deviceKey":"nvx-tx-atv","methodName":"SetIsOnline","params":[false]}</c>.
    /// Offline reports the communication monitor in error; online reports it OK.
    /// </summary>
    public void SetIsOnline(bool state)
    {
        this.LogInformation("MOCK set {state}", state ? "online" : "offline");
        Online = state;
        communicationMonitor.SetOnline(state);
    }

    public void LinkToApi(
        BasicTriList trilist,
        uint joinStart,
        string joinMapKey,
        EiscApiAdvanced bridge
    )
    {
        var joinMap = new NvxDeviceJoinMap(joinStart);

        var deviceBridge = new NvxDeviceBridge(this);
        deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, SetStreamUrl);
    }


    public bool IsEnabled => true;

    public BoolFeedback EnabledFeedback { get; } = new BoolFeedback("EnabledFeedback", () => true);
}
