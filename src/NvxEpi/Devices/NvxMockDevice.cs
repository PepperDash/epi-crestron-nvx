using System;
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

public class NvxMockDevice : ReconfigurableDevice, IStream, ISecondaryAudioStream, IRoutingNumeric, IBridgeAdvanced, IHasFeedback
{
    private NvxMockDeviceProperties properties;
    private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
        new();

    private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
        new();

    private string streamUrl => properties != null && !string.IsNullOrEmpty(properties.StreamUrl) ? properties.StreamUrl : string.Empty;

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
        BuildFeedbacks();
        BuildRoutingPorts();
    }

    protected override void CustomSetConfig(DeviceConfig config)
    {
        var newProperties = config.Properties.ToObject<NvxMockDeviceProperties>();

        if (newProperties == null)
        {
            this.LogError("************ PROPS IS NULL ************");
            throw new NullReferenceException("properties");
        }

        this.LogVerbose("Updating config with {@newConfig}\r\nold {@oldConfig}", newProperties, properties);

        properties = newProperties;

        foreach (var feedback in Feedbacks)
        {
            feedback.FireUpdate();
        }
    }

    private void BuildFeedbacks()
    {
        IsOnline = new BoolFeedback("IsOnline", () => true);
        DeviceMode = new IntFeedback("DeviceMode", () => 0);
        StreamUrl = new StringFeedback("StreamUrl", () => streamUrl);

        MulticastAddress = new StringFeedback("MulticastVideoAddress",
            () => !string.IsNullOrEmpty(properties.MulticastVideoAddress) ? properties.MulticastVideoAddress : string.Empty);

        IsStreamingVideo = new BoolFeedback("isStreamingVideo", () => !string.IsNullOrEmpty(properties.StreamUrl));

        VideoStreamStatus = new StringFeedback("videoStreamStatus",
            () => !string.IsNullOrEmpty(properties.StreamUrl) ? "Streaming" : string.Empty);

        SecondaryAudioAddress = new StringFeedback("secondaryAudioAddress",
            () => !string.IsNullOrEmpty(properties.MulticastAudioAddress) ? properties.MulticastAudioAddress : string.Empty);

        TxAudioAddress = new StringFeedback("txAudioAddress",
            () => !string.IsNullOrEmpty(properties.MulticastAudioAddress) ? properties.MulticastAudioAddress : string.Empty);

        RxAudioAddress = new StringFeedback("rxAudioAddress", () => string.Empty);

        IsStreamingSecondaryAudio = new BoolFeedback("isStreamingSecondaryAudio",
            () => !string.IsNullOrEmpty(properties.MulticastAudioAddress));

        SecondaryAudioStreamStatus = new StringFeedback("secondaryAudioStreamStatus",
            () => !string.IsNullOrEmpty(properties.MulticastAudioAddress) ? "Streaming" : string.Empty);

        SyncDetected = new BoolFeedback("syncDetected", () => Sync);

        Feedbacks.AddRange(new Feedback[]
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
                MulticastAddress
            });
    }

    private void BuildRoutingPorts()
    {
        InputPorts.Add(
            new RoutingInputPort(
                DeviceInputEnum.Hdmi1.Name,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.Hdmi,
                DeviceInputEnum.Hdmi1,
                this));

        InputPorts.Add(
            new RoutingInputPort(
                DeviceInputEnum.SecondaryAudio.Name,
                eRoutingSignalType.Audio | eRoutingSignalType.SecondaryAudio,
                eRoutingPortConnectionType.Streaming,
                DeviceInputEnum.SecondaryAudio,
                this));



        OutputPorts.Add(
            new RoutingOutputPort(
                SwitcherForSecondaryAudioOutput.Key,
                eRoutingSignalType.Audio | eRoutingSignalType.SecondaryAudio,
                eRoutingPortConnectionType.LineAudio,
                null,
                this));

        OutputPorts.Add(new RoutingOutputPort(
            SwitcherForHdmiOutput.Key,
            eRoutingSignalType.AudioVideo,
            eRoutingPortConnectionType.Hdmi,
            null,
            this));


        if (IsTransmitter)
        {
            OutputPorts.Add(
             new RoutingOutputPort(
                 SwitcherForStreamOutput.Key,
                 eRoutingSignalType.AudioVideo,
                 eRoutingPortConnectionType.Streaming,
                 null,
                 this));
        }
        else
        {

            InputPorts.Add(new RoutingInputPort(
            DeviceInputEnum.Stream.Name,
            eRoutingSignalType.AudioVideo,
            eRoutingPortConnectionType.Streaming,
            DeviceInputEnum.Stream,
            this)
            {
                FeedbackMatchObject = eSfpVideoSourceTypes.Stream
            });
        }
    }

    public override bool CustomActivate()
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

    public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
    {
        this.LogVerbose("Executing switch : {0}", signalType);
    }

    public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
    {
        this.LogVerbose("Executing switch : {0}", type);
    }

    public FeedbackCollection<Feedback> Feedbacks { get; private set; }
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
        if (url.Equals(properties.StreamUrl))
            return;

        properties.StreamUrl = url;
        StreamUrl.FireUpdate();
    }

    public void SetSyncState(bool state)
    {
        Sync = state;
    }

    public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        var joinMap = new NvxDeviceJoinMap(joinStart);

        var deviceBridge = new NvxDeviceBridge(this);
        deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, SetStreamUrl);
    }
}