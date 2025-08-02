using System;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Enums;
using NvxEpi.Features.Config;
using NvxEpi.JoinMaps;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.Core.Devices;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Devices;

public class NvxMockDevice : ReconfigurableDevice, IStream, ISecondaryAudioStream, IRoutingNumeric, IBridgeAdvanced
{
    private readonly RoutingPortCollection<RoutingInputPort> _inputPorts =
        new();

    private readonly RoutingPortCollection<RoutingOutputPort> _outputPorts =
        new();

    private string _streamUrl;

    public NvxMockDevice(DeviceConfig dc, bool isTransmitter)
        : base(dc)
    {
        var props = dc.Properties.ToObject<NvxMockDeviceProperties>();
        if (props == null)
        {
            this.LogError("************ PROPS IS NULL ************");
            throw new NullReferenceException("props");
        }

        Feedbacks = new FeedbackCollection<Feedback>();

        DeviceId = props.DeviceId;
        IsTransmitter = isTransmitter;
        _streamUrl = !string.IsNullOrEmpty(props.StreamUrl) ? props.StreamUrl : string.Empty;
        BuildFeedbacks(props);
        BuildRoutingPorts();
    }

    private void BuildFeedbacks(NvxMockDeviceProperties props)
    {
        IsOnline = new BoolFeedback("IsOnline", () => true);
        DeviceMode = new IntFeedback("DeviceMode", () => 0);
        StreamUrl = new StringFeedback("StreamUrl", () => _streamUrl);

        MulticastAddress = new StringFeedback("MulticastVideoAddress",
            () => !string.IsNullOrEmpty(props.MulticastVideoAddress) ? props.MulticastVideoAddress : string.Empty);

        IsStreamingVideo = new BoolFeedback(() => !string.IsNullOrEmpty(props.StreamUrl));

        VideoStreamStatus = new StringFeedback(
            () => !string.IsNullOrEmpty(props.StreamUrl) ? "Streaming" : string.Empty);

        SecondaryAudioAddress = new StringFeedback(
            () => !string.IsNullOrEmpty(props.MulticastAudioAddress) ? props.MulticastAudioAddress : string.Empty);

        TxAudioAddress = new StringFeedback("MulticastAudio",
            () => !string.IsNullOrEmpty(props.MulticastAudioAddress) ? props.MulticastAudioAddress : string.Empty);

        RxAudioAddress = new StringFeedback(() => string.Empty);

        IsStreamingSecondaryAudio = new BoolFeedback(
            () => !string.IsNullOrEmpty(props.MulticastAudioAddress));

        SecondaryAudioStreamStatus = new StringFeedback(
            () => !string.IsNullOrEmpty(props.MulticastAudioAddress) ? "Streaming" : string.Empty);

        Feedbacks.AddRange(new Feedback[]
            {
                DeviceNameFeedback.GetFeedback(Name),
                IsOnline,
                StreamUrl,
                MulticastAddress,
                TxAudioAddress
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
        Debug.Console(2, this, "Executing switch : {0}", signalType);
    }

    public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
    {
        Debug.Console(2, this, "Executing switch : {0}", type);
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

    private void SetStreamUrl(string url)
    {
        if (url.Equals(_streamUrl))
            return;

        _streamUrl = url;
        StreamUrl.FireUpdate();

        /*foreach (
            var rx in
                DeviceManager.AllDevices.OfType<IStreamWithHardware>()
                             .Where(x => !x.IsTransmitter && x.StreamUrl.StringValue.Equals(oldUrl)))
            rx.RouteStream(this);*/
    }

    public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
    {
        var joinMap = new NvxDeviceJoinMap(joinStart);

        var deviceBridge = new NvxDeviceBridge(this);
        deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, SetStreamUrl);
    }
}