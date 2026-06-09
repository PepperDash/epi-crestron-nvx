using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NvxEpi
{
    public class NvxDevice : EssentialsDevice, INvxDevice, INvxNetworkPortInformation, IRoutingWithFeedback, IHasFeedback, ICommunicationMonitor, IBridgeAdvanced, IComPorts, IIROutputPorts
    {
        public enum NvxInputSelector
        {
            Stream,
            DmNax,
            Hdmi1,
            Hdmi2,
            UsbC1,
            UsbC2,
            AnalogAudio,
            DanteAes67,
            DM,
            Arc,
            Bts,
            Noop
        }

        public enum NvxOutputSelector
        {
            Stream,
            DmNax,
            Hdmi,
            AnalogAudio,
            DanteAes67,
            Arc,
            Bts,
            Noop
        }

        public const string StreamRoutingPortKey = "stream";
        public const string DmNaxRoutingPortKey = "dmNax";
        public const string HdmiOutRoutingPortKey = "hdmi";
        public const string Hdmi1RoutingPortKey = "hdmi1";
        public const string Hdmi2RoutingPortKey = "hdmi2";
        public const string UsbCIn1RoutingPortKey = "usbCIn1";
        public const string UsbCIn2RoutingPortKey = "usbCIn2";
        public const string AnalogAudioRoutingPortKey = "analogAudio";
        public const string DanteAes67RoutingPortKey = "dante_aes67";
        public const string DmInRoutingPortKey = "dm";
        public const string ArcRoutingPortKey = "arc";
        public const string BtsRoutingPortKey = "bts";
        public const string AudioLoopOutRoutingPortKey = "loopOut";

        private readonly NvxDeviceProperties props;
        private readonly DmNvxBaseClass device;
        private readonly NvxNetworkPortInfo networkPortInfo;

        public bool IsTransmitter { get; }

        public NvxDevice(string key, NvxDeviceProperties props, DmNvxBaseClass device) : base(key, device.Name)
        {
            this.props = props ?? throw new ArgumentNullException(nameof(props));
            this.device = device ?? throw new ArgumentNullException(nameof(device));

            if (props.DeviceId == 0)
            {
                throw new ArgumentException("DeviceId must be set and non-zero");
            }

            networkPortInfo = new NvxNetworkPortInfo(key, device);

            CommunicationMonitor = new CrestronGenericBaseCommunicationMonitor(this, device, 10000, 30000);
            IsTransmitter = props.Mode.ToLowerInvariant() == "tx";

            if (IsTransmitter)
            {
                OutputPorts.Add(new RoutingOutputPort(StreamRoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, NvxOutputSelector.Stream, this));
            }
            else
            {
                InputPorts.Add(new RoutingInputPort(StreamRoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, NvxInputSelector.Stream, this));
            }

            if (device is DmNvx363 or DmNvx363C)
            {
                InputPorts.Add(new RoutingInputPort(DanteAes67RoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxInputSelector.DanteAes67, this));
                InputPorts.Add(new RoutingInputPort(DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxInputSelector.DmNax, this));
                OutputPorts.Add(new RoutingOutputPort(DanteAes67RoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DanteAes67, this));
                OutputPorts.Add(new RoutingOutputPort(DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DmNax, this));
            }
            else if (device is DmNvxE3x or DmNvxE10E20Base or DmNvxE760x)
            {
                OutputPorts.Add(new RoutingOutputPort(DanteAes67RoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DanteAes67, this));
                OutputPorts.Add(new RoutingOutputPort(DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DmNax, this));
            }
            else if (device is DmNvxD3x or DmNvxD10D20Base or DmNvxD200)
            {
                InputPorts.Add(new RoutingInputPort(DanteAes67RoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DanteAes67, this));
                InputPorts.Add(new RoutingInputPort(DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxInputSelector.DmNax, this));
            }
            else
            {
                InputPorts.Add(new RoutingInputPort(DanteAes67RoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxInputSelector.DanteAes67, this));
                InputPorts.Add(new RoutingInputPort(DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxInputSelector.DmNax, this));
                OutputPorts.Add(new RoutingOutputPort(DanteAes67RoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DanteAes67, this));
                OutputPorts.Add(new RoutingOutputPort(DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, NvxOutputSelector.DmNax, this));
            }

            switch (device)
            {
                case DmNvx35x:
                    InputPorts.Add(HdmiInput1InputPort(device, this));
                    InputPorts.Add(HdmiInput2InputPort(device, this));
                    InputPorts.Add(new RoutingInputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxInputSelector.AnalogAudio, this));

                    OutputPorts.Add(HdmiOutputPort(device, this));
                    OutputPorts.Add(new RoutingOutputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxOutputSelector.AnalogAudio, this));
                    break;

                case DmNvx36x:
                    InputPorts.Add(HdmiInput1InputPort(device, this));
                    InputPorts.Add(new RoutingInputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxInputSelector.AnalogAudio, this));

                    OutputPorts.Add(HdmiOutputPort(device, this));
                    OutputPorts.Add(new RoutingOutputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxOutputSelector.AnalogAudio, this));
                    break;

                case DmNvx38x:
                    InputPorts.Add(HdmiInput1InputPort(device, this));
                    InputPorts.Add(HdmiInput2InputPort(device, this));
                    InputPorts.Add(new RoutingInputPort(UsbCIn1RoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.UsbC, NvxInputSelector.UsbC1, this));
                    InputPorts.Add(new RoutingInputPort(UsbCIn2RoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.UsbC, NvxInputSelector.UsbC2, this));
                    InputPorts.Add(new RoutingInputPort(BtsRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Hdmi, NvxInputSelector.Bts, this));
                    InputPorts.Add(new RoutingInputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxInputSelector.AnalogAudio, this));
                    InputPorts.Add(new RoutingInputPort(ArcRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Hdmi, NvxInputSelector.Arc, this));

                    OutputPorts.Add(HdmiOutputPort(device, this));
                    OutputPorts.Add(new RoutingOutputPort(ArcRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Hdmi, NvxOutputSelector.Arc, this));
                    OutputPorts.Add(new RoutingOutputPort(BtsRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Hdmi, NvxOutputSelector.Bts, this));
                    OutputPorts.Add(new RoutingOutputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxOutputSelector.AnalogAudio, this));
                    break;

                case DmNvxE760x:
                    InputPorts.Add(DmInputPort(device, this));
                    InputPorts.Add(new RoutingInputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxInputSelector.AnalogAudio, this));
                    break;

                case DmNvxE3x:
                    InputPorts.Add(HdmiInput1InputPort(device, this));
                    InputPorts.Add(new RoutingInputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxInputSelector.AnalogAudio, this));
                    break;

                case DmNvxE10E20Base:
                    InputPorts.Add(HdmiInput1InputPort(device, this));
                    OutputPorts.Add(new RoutingOutputPort(AudioLoopOutRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxOutputSelector.Noop, this));
                    break;

                case DmNvxD3x:
                    OutputPorts.Add(HdmiOutputPort(device, this));
                    OutputPorts.Add(new RoutingOutputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxOutputSelector.AnalogAudio, this));
                    break;

                case DmNvxD10D20Base:
                    OutputPorts.Add(HdmiOutputPort(device, this));
                    OutputPorts.Add(new RoutingOutputPort(AnalogAudioRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.LineAudio, NvxOutputSelector.AnalogAudio, this));
                    break;

                default:
                    throw new Exception("Unsupported device type: " + device?.GetType().Name);
            }

            StreamUrlFeedback = new StringFeedback("StreamUrl", () => device.Control.ServerUrlFeedback.StringValue);
            Feedbacks.Add(StreamUrlFeedback);
            StreamUrlFeedback.OutputChange += (s, a) => this.LogInformation("Stream URL changed to {StreamUrl}", a.StringValue);

            IsStreamingFeedback = new BoolFeedback("IsStreaming", () => device.Control.StartFeedback.BoolValue);
            Feedbacks.Add(IsStreamingFeedback);

            StreamStatusFeedback = new StringFeedback("StreamStatus", () => device.Control.StatusTextFeedback.StringValue);
            Feedbacks.Add(StreamStatusFeedback);
            StreamStatusFeedback.OutputChange += (s, a) => this.LogInformation("Stream status changed to {StreamStatus}", a.StringValue);

            DmNaxTxAddressFeedback = new StringFeedback("DmNaxTxAddress", () => device.DmNaxRouting.DmNaxTransmit.MulticastAddressFeedback.StringValue);
            Feedbacks.Add(DmNaxTxAddressFeedback);
            DmNaxTxAddressFeedback.OutputChange += (s, a) => this.LogInformation("DM NAX TX Address changed to {DmNaxTxAddress}", a.StringValue);

            IsTransmittingDmNaxFeedback = new BoolFeedback("IsTransmittingDmNax",
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback == DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);
            Feedbacks.Add(IsTransmittingDmNaxFeedback);

            DmNaxTransmitStatusFeedback = new StringFeedback("DmNaxTransmitStatus", () => device.DmNaxRouting.DmNaxTransmit.StreamStatusFeedback.ToString());
            Feedbacks.Add(DmNaxTransmitStatusFeedback);
            DmNaxTransmitStatusFeedback.OutputChange += (s, a) => this.LogInformation("DM NAX transmit stream status changed to {DmNaxTransmitStatus}", a.StringValue);

            DmNaxRxAddressFeedback = new StringFeedback("DmNaxRxAddress", () => device.DmNaxRouting.DmNaxReceive.MulticastAddressFeedback.StringValue);
            Feedbacks.Add(DmNaxRxAddressFeedback);
            DmNaxRxAddressFeedback.OutputChange += (s, a) => this.LogInformation("DM NAX RX Address changed to {DmNaxRxAddress}", a.StringValue);

            IsReceivingDmNaxFeedback = new BoolFeedback("IsReceivingDmNax",
                () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback == DmNvxBaseClass.DmNvx35xDmNaxTransmitReceiveBase.eStreamStatus.StreamStarted);
            Feedbacks.Add(IsReceivingDmNaxFeedback);

            DmNaxReceiveStatusFeedback = new StringFeedback("DmNaxReceiveStatus", () => device.DmNaxRouting.DmNaxReceive.StreamStatusFeedback.ToString());
            Feedbacks.Add(DmNaxReceiveStatusFeedback);
            DmNaxReceiveStatusFeedback.OutputChange += (s, a) => this.LogInformation("DM NAX receive stream status changed to {DmNaxReceiveStatus}", a.StringValue);

            MulticastAddressFeedback = new StringFeedback("MulticastAddress", () => device.Control.MulticastAddressFeedback.StringValue);
            Feedbacks.Add(MulticastAddressFeedback);
            MulticastAddressFeedback.OutputChange += (s, e) => this.LogInformation("Multicast address changed to {MulticastAddress}", e.StringValue);

            device.OnlineStatusChange += DeviceOnOnlineStatusChange;
            device.BaseEvent += DeviceOnBaseEvent;
            device.DmNaxRouting.DmNaxRoutingChange += DmNaxRoutingOnRoutingChange;
            device.DmNaxRouting.DmNaxTransmit.DmNaxStreamChange += DmNaxTransmitOnDmNaxStreamChange;
            device.DmNaxRouting.DmNaxReceive.DmNaxStreamChange += DmNaxReceiveOnDmNaxStreamChange; ;

            if (device.SourceReceive != null)
            {
                device.SourceReceive.StreamChange += (stream, args) => StreamUrlFeedback.FireUpdate();
            }

            if (device.SourceTransmit != null)
            {
                device.SourceTransmit.StreamChange += (stream, args) => StreamUrlFeedback.FireUpdate();
            }

            if (IsTransmitter)
            {
                NvxRouter.RegisterTransmitter(this);
            }
            else
            {
                NvxRouter.RegisterReceiver(this);
            }

            foreach (var input in InputPorts)
            {
                this.LogInformation("Added input port {PortKey}", input.Key);
            }

            foreach (var output in OutputPorts)
            {
                this.LogInformation("Added output port {PortKey}", output.Key);
            }
        }

        private void DmNaxReceiveOnDmNaxStreamChange(object sender, GenericEventArgs args)
        {
            DmNaxRxAddressFeedback.FireUpdate();
            DmNaxReceiveStatusFeedback.FireUpdate();
            IsReceivingDmNaxFeedback.FireUpdate();
        }

        private void DmNaxTransmitOnDmNaxStreamChange(object sender, GenericEventArgs args)
        {
            DmNaxTxAddressFeedback.FireUpdate();
            DmNaxTransmitStatusFeedback.FireUpdate();
            IsTransmittingDmNaxFeedback.FireUpdate();
        }

        private void DmNaxRoutingOnRoutingChange(object sender, GenericEventArgs args)
        {
            DmNaxTxAddressFeedback.FireUpdate();
            DmNaxRxAddressFeedback.FireUpdate();
        }

        private void DeviceOnOnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            if (!args.DeviceOnLine)
            {
                return;
            }

            foreach (var feedback in Feedbacks)
            {
                feedback.FireUpdate();
            }
        }

        private void DeviceOnBaseEvent(GenericBase device, BaseEventArgs args)
        {
            switch (args.EventId)
            {
                case DMInputEventIds.StatusTextEventId:
                    this.LogDebug("Status changed to {statusText}", this.device.Control.StatusTextFeedback);
                    break;

                case DMInputEventIds.NameFeedbackEventId:
                    Name = device.Name;
                    break;

                case DMInputEventIds.ActiveVideoSourceEventId:
                    this.LogDebug("Active video source changed to {source}, updating route feedback", this.device.Control.ActiveVideoSourceFeedback);
                    OnActiveVideoSourceFeedbackChanged();
                    break;

                case DMInputEventIds.ActiveAudioSourceEventId:
                    this.LogDebug("Active audio source changed to {source}, updating route feedback", this.device.Control.ActiveAudioSourceFeedback);
                    OnActiveAudioSourceFeedbackChanged();
                    break;

                case DMInputEventIds.ActiveDanteAudioSourceEventId:
                    this.LogDebug("Active Dante/AES67 transmit audio source changed to {source}, updating route feedback", this.device.Control.ActiveDanteAudioSourceFeedback);
                    OnActiveDanteAudioSourceFeedbackChanged();
                    break;

                case DMInputEventIds.ActiveDmNaxAudioSourceFeedbackEventId:
                    this.LogDebug("Active DM NAX transmit audio source changed to {source}, updating route feedback", this.device.Control.ActiveDmNaxAudioSourceFeedback);
                    OnActiveDmNaxAudioSourceFeedbackChanged();
                    break;

                case DMInputEventIds.ActiveeArcAudioSourceFeedbackEventId:
                    this.LogDebug("Active eARC audio source changed to {source}, updating route feedback", this.device.Control.ActiveeArcAudioSourceFeedback);
                    OnActiveArcAudioSourceFeedbackChanged();
                    break;

                case DMInputEventIds.ActiveBtsAudioSourceFeedbackEventId:
                    this.LogDebug("Active BTS audio source changed to {source}, updating route feedback", this.device.Control.ActiveBtsAudioSourceFeedback);
                    OnActiveBtsAudioSourceFeedbackChanged();
                    break;

                case DMInputEventIds.StreamUriFeedbackEventId:
                    this.LogDebug("Stream URI changed to {streamUri} from {event}, updating stream URL feedback", this.device.Control.ServerUrlFeedback, nameof(device.BaseEvent));
                    StreamUrlFeedback.FireUpdate();
                    break;

                case DMInputEventIds.MulticastAddressEventId:
                    this.LogDebug("Multicast address changed to {multicastAddress} from {event}, updating multicast address feedback", this.device.Control.MulticastAddressFeedback, nameof(device.BaseEvent));
                    MulticastAddressFeedback.FireUpdate();
                    break;
            }
        }

        private void OnActiveVideoSourceFeedbackChanged()
        {
            foreach (var videoRoute in GetVideoRoutes())
            {
                RouteChanged?.Invoke(this, videoRoute);
            }
        }

        private void OnActiveAudioSourceFeedbackChanged()
        {
            var audioInput = GetCurrentAudioRoutingInput();
            if (audioInput == null)
            {
                return;
            }

            var route = GetAnalogAudioRouteDescriptor(audioInput);
            if (route != null)
            {
                RouteChanged?.Invoke(this, route);
            }
        }

        private void OnActiveDanteAudioSourceFeedbackChanged()
        {
            var audioInput = GetCurrentAudioRoutingInput();
            if (audioInput == null)
            {
                return;
            }

            var route = GetDanteAes67RouteDescriptor(audioInput);
            if (route != null)
            {
                RouteChanged?.Invoke(this, route);
            }
        }

        private void OnActiveDmNaxAudioSourceFeedbackChanged()
        {
            var audioInput = GetCurrentAudioRoutingInput();
            if (audioInput == null)
            {
                return;
            }

            var route = GetDmNaxRouteDescriptor(audioInput);
            if (route != null)
            {
                RouteChanged?.Invoke(this, route);
            }
        }

        private void OnActiveBtsAudioSourceFeedbackChanged()
        {
            var audioInput = GetCurrentAudioRoutingInput();
            if (audioInput == null)
            {
                return;
            }

            var route = GetBtsRouteDescriptor(audioInput);
            if (route != null)
            {
                RouteChanged?.Invoke(this, route);
            }
        }

        private void OnActiveArcAudioSourceFeedbackChanged()
        {
            var audioInput = GetCurrentAudioRoutingInput();
            if (audioInput == null)
            {
                return;
            }

            var route = GetArcRouteDescriptor(audioInput);
            if (route != null)
            {
                RouteChanged?.Invoke(this, route);
            }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; } = new();
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; } = new();

        public List<RouteSwitchDescriptor> CurrentRoutes => GetRoutes().ToList();

        public event RouteChangedEventHandler? RouteChanged;

        public event EventHandler? PortInformationChanged
        {
            add => networkPortInfo.PortInformationChanged += value;
            remove => networkPortInfo.PortInformationChanged -= value;
        }

        public FeedbackCollection<PepperDash.Essentials.Core.Feedback> Feedbacks { get; } = new FeedbackCollection<PepperDash.Essentials.Core.Feedback>();

        public StatusMonitorBase? CommunicationMonitor { get; private set; }

        public StringFeedback StreamUrlFeedback { get; }

        public BoolFeedback IsStreamingFeedback { get; }

        public StringFeedback StreamStatusFeedback { get; }

        public StringFeedback DmNaxTxAddressFeedback { get; }

        public BoolFeedback IsTransmittingDmNaxFeedback { get; }

        public StringFeedback DmNaxTransmitStatusFeedback { get; }

        public BoolFeedback IsReceivingDmNaxFeedback { get; }

        public StringFeedback DmNaxReceiveStatusFeedback { get; }

        public StringFeedback DmNaxRxAddressFeedback { get; }

        public StringFeedback MulticastAddressFeedback { get; }

        public int DeviceId => props.DeviceId;

        public CrestronCollection<ComPort> ComPorts => device.ComPorts;

        public int NumberOfComPorts => device.NumberOfComPorts;

        public CrestronCollection<IROutputPort> IROutputPorts => device.IROutputPorts;

        public int NumberOfIROutputPorts => device.NumberOfIROutputPorts;

        public List<NvxNetworkPortInformation> NetworkPorts => networkPortInfo.NetworkPorts;

        public virtual void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            this.LogInformation("Attemping to route {InputSelector} to {OutputSelector} with route type {RouteType}", inputSelector, outputSelector, signalType);
            switch (signalType)
            {
                case eRoutingSignalType.AudioVideo:
                    ExecuteSwitch(inputSelector, outputSelector, eRoutingSignalType.Video);
                    ExecuteSwitch(inputSelector, outputSelector, eRoutingSignalType.Audio);
                    break;
                case eRoutingSignalType.Video when outputSelector is NvxOutputSelector videoOutput && inputSelector is NvxInputSelector videoInput:
                    switch (videoOutput)
                    {
                        // Both of these cases are the same, duplicated for posterity.  If the device is a decoder, it will route to the HDMI output, if it's an encoder, it will route to the stream output.
                        case NvxOutputSelector.Hdmi:
                            switch (videoInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Stream;
                                    break;
                                case NvxInputSelector.UsbC1:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Usbc1;
                                    break;
                                case NvxInputSelector.UsbC2:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Usbc2;
                                    break;
                                case NvxInputSelector.DmNax:
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    break;
                                case NvxInputSelector.DM:
                                    break;
                                case NvxInputSelector.Bts:
                                    break;
                            }
                            break;
                        case NvxOutputSelector.Stream:
                            switch (videoInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Stream;
                                    break;
                                case NvxInputSelector.UsbC1:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Usbc1;
                                    break;
                                case NvxInputSelector.UsbC2:
                                    device.Control.VideoSource = eSfpVideoSourceTypes.Usbc2;
                                    break;
                                case NvxInputSelector.DM:
                                    break;
                                case NvxInputSelector.DmNax:
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    break;
                                case NvxInputSelector.Bts:
                                    break;
                            }
                            break;
                        default:
                            this.LogWarning("Unsupported video output selector: {VideoOutput}", videoOutput);
                            break;
                    }
                    break;
                case eRoutingSignalType.Audio when outputSelector is NvxOutputSelector audioOutput && inputSelector is NvxInputSelector audioInput:
                    switch (audioOutput)
                    {
                        case NvxOutputSelector.Hdmi:
                            switch (audioInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                                    break;
                                case NvxInputSelector.DmNax:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio;
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.DanteAes67Audio;
                                    break;
                                case NvxInputSelector.DM:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Arc:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.eArc;
                                    break;
                                case NvxInputSelector.Bts:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Bts;
                                    break;
                                default:
                                    this.LogError("Unsupported audio input selector for DmNax output: {AudioInput}", audioInput);
                                    break;
                            }
                            break;
                        case NvxOutputSelector.Stream:
                            switch (audioInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Input2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio;
                                    break;
                                case NvxInputSelector.DmNax:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.DanteAes67Audio;
                                    break;
                                case NvxInputSelector.DM:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Arc:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.eArc;
                                    break;
                                case NvxInputSelector.Bts:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Bts;
                                    break;
                                default:
                                    this.LogError("Unsupported audio input selector for DmNax output: {AudioInput}", audioInput);
                                    break;
                            }
                            break;
                        case NvxOutputSelector.DmNax:
                            switch (audioInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Input2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                                    break;
                                case NvxInputSelector.DmNax:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.DanteAes67Audio;
                                    break;
                                case NvxInputSelector.UsbC1:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Usbc1;
                                    break;
                                case NvxInputSelector.UsbC2:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Usbc2;
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
                                    break;
                                case NvxInputSelector.Arc:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.eArc;
                                    break;
                                case NvxInputSelector.DM:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Bts:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.Bts;
                                    break;
                                default:
                                    this.LogError("Unsupported audio input selector for DmNax output: {AudioInput}", audioInput);
                                    break;
                            }
                            break;
                        case NvxOutputSelector.DanteAes67:
                            switch (audioInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                                    break;
                                case NvxInputSelector.DmNax:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.DanteAes67Audio;
                                    break;
                                case NvxInputSelector.UsbC1:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Usbc1;
                                    break;
                                case NvxInputSelector.UsbC2:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.Usbc2;
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    device.Control.DmNaxAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
                                    break;
                                case NvxInputSelector.Arc:
                                    device.Control.AudioSource = DmNvxControl.eAudioSource.eArc;
                                    break;
                                case NvxInputSelector.DM:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Bts:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Bts;
                                    break;
                                default:
                                    this.LogError("Unsupported audio input selector for Dante output: {AudioInput}", audioInput);
                                    break;
                            }
                            break;
                        case NvxOutputSelector.Arc:
                            switch (audioInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.Input2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                                    break;
                                case NvxInputSelector.DmNax:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.DanteAes67Audio;
                                    break;
                                case NvxInputSelector.Arc:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.eArc;
                                    break;
                                case NvxInputSelector.DM:
                                    device.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Bts:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.Bts;
                                    break;
                                default:
                                    this.LogError("Unsupported audio input selector for DmNax output: {AudioInput}", audioInput);
                                    break;
                            }
                            break;
                        case NvxOutputSelector.Bts:
                            switch (audioInput)
                            {
                                case NvxInputSelector.Hdmi1:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.Hdmi2:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.Input2;
                                    break;
                                case NvxInputSelector.Stream:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
                                    break;
                                case NvxInputSelector.DmNax:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
                                    break;
                                case NvxInputSelector.AnalogAudio:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
                                    break;
                                case NvxInputSelector.DM:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.Input1;
                                    break;
                                case NvxInputSelector.DanteAes67:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.DanteAes67Audio;
                                    break;
                                case NvxInputSelector.Arc:
                                    device.Control.BtsAudioSource = DmNvxControl.eAudioSource.eArc;
                                    break;
                                case NvxInputSelector.Bts:
                                    device.Control.eArcAudioSource = DmNvxControl.eAudioSource.Bts;
                                    break;
                                default:
                                    this.LogError("Unsupported audio input selector for DmNax output: {AudioInput}", audioInput);
                                    break;
                            }
                            break;
                        case NvxOutputSelector.Noop:
                            break;
                        default:
                            this.LogError("Unsupported audio output selector for Dante output: {AudioOutput}", audioOutput);
                            break;
                    }
                    break;

                default:
                    this.LogWarning("Unsupported route type {RouteType} for input selector {InputSelector} and output selector {OutputSelector}", signalType, inputSelector, outputSelector);
                    break;
            }
        }

        protected virtual IEnumerable<RouteSwitchDescriptor> GetRoutes()
        {
            foreach (var route in GetVideoRoutes())
            {
                yield return route;
            }

            foreach (var route in GetAudioRoutes())
            {
                yield return route;
            }
        }

        private IEnumerable<RouteSwitchDescriptor> GetVideoRoutes()
        {
            var videoSource = GetCurrentVideoRoutingInput();
            if (videoSource == null)
            {
                yield break;
            }

            var hdmiOutput = OutputPorts[HdmiOutRoutingPortKey];
            if (hdmiOutput == null)
            {
                yield return new RouteSwitchDescriptor(outputPort: OutputPorts[HdmiOutRoutingPortKey], inputPort: videoSource);
            }

            var streamOutput = OutputPorts[StreamRoutingPortKey];
            if (streamOutput == null)
            {
                yield return new RouteSwitchDescriptor(outputPort: OutputPorts[StreamRoutingPortKey], inputPort: videoSource);
            }

            var audioLoopOutput = OutputPorts[AudioLoopOutRoutingPortKey];
            if (audioLoopOutput == null)
            {
                yield return new RouteSwitchDescriptor(outputPort: OutputPorts[AudioLoopOutRoutingPortKey], inputPort: videoSource);
            }
        }

        private RoutingInputPort? GetCurrentVideoRoutingInput() =>
            device.Control.ActiveVideoSourceFeedback switch
            {
                eSfpVideoSourceTypes.Hdmi1 => InputPorts[Hdmi1RoutingPortKey],
                eSfpVideoSourceTypes.Hdmi2 => InputPorts[Hdmi2RoutingPortKey],
                eSfpVideoSourceTypes.Stream => InputPorts[StreamRoutingPortKey],
                eSfpVideoSourceTypes.Usbc1 => InputPorts[UsbCIn1RoutingPortKey],
                eSfpVideoSourceTypes.Usbc2 => InputPorts[UsbCIn2RoutingPortKey],
                _ => default
            };

        private IEnumerable<RouteSwitchDescriptor> GetAudioRoutes()
        {
            var audioSource = GetCurrentAudioRoutingInput();
            if (audioSource == null)
            {
                yield break;
            }

            if (GetAnalogAudioRouteDescriptor(audioSource) is { } analogAudioRoute)
            {
                yield return analogAudioRoute;
            }

            if (GetDanteAes67RouteDescriptor(audioSource) is { } danteAes67AudioRoute)
            {
                yield return danteAes67AudioRoute;
            }

            if (GetDmNaxRouteDescriptor(audioSource) is { } dmNaxAudioRoute)
            {
                yield return dmNaxAudioRoute;
            }
        }

        private RoutingInputPort? GetCurrentAudioRoutingInput() =>
            device.Control.ActiveAudioSourceFeedback switch
            {
                DmNvxControl.eAudioSource.Input1 => InputPorts[Hdmi1RoutingPortKey],
                DmNvxControl.eAudioSource.Input2 => InputPorts[Hdmi2RoutingPortKey],
                DmNvxControl.eAudioSource.PrimaryStreamAudio => InputPorts[StreamRoutingPortKey],
                DmNvxControl.eAudioSource.AnalogAudio => InputPorts[AnalogAudioRoutingPortKey],
                DmNvxControl.eAudioSource.DanteAes67Audio => InputPorts[DanteAes67RoutingPortKey],
                DmNvxControl.eAudioSource.eArc => InputPorts[ArcRoutingPortKey],
                _ => default
            };

        private RouteSwitchDescriptor? GetAnalogAudioRouteDescriptor(RoutingInputPort inputPort) =>
            OutputPorts[AnalogAudioRoutingPortKey] is { } outputPort
                ? new RouteSwitchDescriptor(outputPort: outputPort, inputPort: inputPort)
                : default;

        private RouteSwitchDescriptor? GetDanteAes67RouteDescriptor(RoutingInputPort inputPort) =>
            OutputPorts[DanteAes67RoutingPortKey] is { } outputPort
                ? new RouteSwitchDescriptor(outputPort: outputPort, inputPort: inputPort)
                : default;

        private RouteSwitchDescriptor? GetDmNaxRouteDescriptor(RoutingInputPort inputPort) =>
            OutputPorts[DmNaxRoutingPortKey] is { } outputPort
                ? new RouteSwitchDescriptor(outputPort: outputPort, inputPort: inputPort)
                : default;

        private RouteSwitchDescriptor? GetBtsRouteDescriptor(RoutingInputPort inputPort) =>
            OutputPorts[BtsRoutingPortKey] is { } outputPort
                ? new RouteSwitchDescriptor(outputPort: outputPort, inputPort: inputPort)
                : default;

        private RouteSwitchDescriptor? GetArcRouteDescriptor(RoutingInputPort inputPort) =>
            OutputPorts[ArcRoutingPortKey] is { } outputPort
                ? new RouteSwitchDescriptor(outputPort: outputPort, inputPort: inputPort)
                : default;

        private static RoutingInputPort HdmiInput1InputPort(DmNvxBaseClass device, INvxDevice parent) => HdmiInputInputPort(device, parent, 1);
        private static RoutingInputPort HdmiInput2InputPort(DmNvxBaseClass device, INvxDevice parent) => HdmiInputInputPort(device, parent, 2);

        private static RoutingInputPort HdmiInputInputPort(DmNvxBaseClass device, INvxDevice parent, uint index)
        {
            var hdmi = device.HdmiIn[index];
            if (hdmi == null)
            {
                parent.LogWarning("Device {DeviceKey} is missing expected HDMI input at index {InputIndex}", parent.Key, index);
                throw new Exception($"Device is missing expected HDMI input at index {index}");
            }

            var key = index switch
            {
                1 => Hdmi1RoutingPortKey,
                2 => Hdmi2RoutingPortKey,
                _ => throw new ArgumentOutOfRangeException(nameof(index), $"Unsupported HDMI input index: {index}")
            };

            var selector = index switch
            {
                1 => NvxInputSelector.Hdmi1,
                2 => NvxInputSelector.Hdmi2,
                _ => throw new ArgumentOutOfRangeException(nameof(index), $"Unsupported HDMI input index: {index}")
            };

            var hdcpCapability = new StringFeedback("hdcpCapability", () => hdmi.HdcpCapabilityFeedback.ToString());
            var audioChannelCount = new IntFeedback("audioChannelCount", () => hdmi.AudioChannelsFeedback.UShortValue);
            var audioFormat = new StringFeedback("audioFormat", () => hdmi.AudioFormatFeedback.ToString());
            var colorspaceMode = new StringFeedback("colorspaceMode", () => hdmi.VideoAttributes.ColorSpaceFeedback.ToString());
            var hdrType = new StringFeedback("hdrType", () => hdmi.HdrTypeFeedback.ToString());

            var port = new NvxHdmiInputPort(
                key,
                parent,
                selector,
                hdcpCapability,
                audioChannelCount,
                audioFormat,
                colorspaceMode,
                hdrType,
                new VideoStatusFuncsWrapper()
                {
                    HasVideoStatusFunc = () => true,
                    VideoSyncFeedbackFunc = () => hdmi.SyncDetectedFeedback.BoolValue,
                    VideoResolutionFeedbackFunc = () => hdmi.VideoAttributes.HorizontalResolutionFeedback.StringValue + "x" + hdmi.VideoAttributes.VerticalResolutionFeedback.StringValue,
                    HdcpStateFeedbackFunc = () => hdmi.HdcpCapabilityFeedback.ToString(),
                    // TODO: HDCP status feedback
                });

            hdmi.StreamChange += (s, e) =>
            {
                port.VideoStatus.FireAll();
                hdcpCapability.FireUpdate();
                audioChannelCount.FireUpdate();
                audioFormat.FireUpdate();
                colorspaceMode.FireUpdate();
                hdrType.FireUpdate();
            };

            hdmi.VideoAttributes.AttributeChange += (s, e) =>
            {
                port.VideoStatus.FireAll();
                colorspaceMode.FireUpdate();
            };

            return port;
        }

        private static RoutingOutputPort HdmiOutputPort(DmNvxBaseClass device, INvxDevice parent)
        {
            var hdmi = device.HdmiOut;
            if (hdmi == null)
            {
                parent.LogWarning("Device {DeviceKey} is missing expected HDMI output", parent.Key);
                throw new Exception("Device is missing expected HDMI output");
            }

            var disabledByHdpcFeedback = new BoolFeedback("disabledByHdcp", () => hdmi.DisabledByHdcpFeedback.BoolValue);
            var outputResolutionFeedback = new StringFeedback("outputResolution", () => hdmi.ResolutionFeedback.ToString());
            var edidManufacturerFeedback = new StringFeedback("edidManufacturer", () => hdmi.EdidFeedback.StringValue);

            var port = new NvxHdmiOutputPort(
                HdmiOutRoutingPortKey,
                parent,
                disabledByHdpcFeedback,
                outputResolutionFeedback,
                edidManufacturerFeedback);

            hdmi.StreamChange += (s, e) =>
            {
                disabledByHdpcFeedback.FireUpdate();
                outputResolutionFeedback.FireUpdate();
                edidManufacturerFeedback.FireUpdate();
            };

            return port;
        }

        private static RoutingInputPort DmInputPort(DmNvxBaseClass device, IRoutingInputs parent)
        {
            var dm = device.DmIn;
            if (dm == null)
            {
                parent.LogWarning("Device {DeviceKey} is missing expected DM input", parent.Key);
                throw new Exception("Device is missing expected DM input");
            }

            var port = new RoutingInputPortWithVideoStatuses(
                DmInRoutingPortKey,
                eRoutingSignalType.AudioVideo,
                eRoutingPortConnectionType.DmCat,
                NvxInputSelector.DM,
                parent,
                new VideoStatusFuncsWrapper()
                {
                    HasVideoStatusFunc = () => true,
                    VideoSyncFeedbackFunc = () => dm.SyncDetectedFeedback.BoolValue,
                    VideoResolutionFeedbackFunc = () => dm.VideoAttributes.HorizontalResolutionFeedback.StringValue + "x" + dm.VideoAttributes.VerticalResolutionFeedback.StringValue,
                    // TODO: HDCP status feedback
                });

            dm.InputStreamChange += (s, e) => port.VideoStatus.FireAll();
            return port;
        }

        public override void Initialize() =>
            CrestronInvoke.BeginInvoke(_ =>
            {
                if (device.Registerable)
                {
                    var result = device.RegisterWithLogging(Key);

                    if (result != eDeviceRegistrationUnRegistrationResponse.Success)
                    {
                        this.LogError("Failed to register XIO Director: {result} {reason}", result, device.RegistrationFailureReason);
                    }
                }
            });

        public void SetIncomingStreamUrl(string streamUrl)
        {
            if (IsTransmitter)
            {
                this.LogInformation("Cannot set incoming stream url on a transmitter device");
                return;
            }

            device.Control.ServerUrl.StringValue = streamUrl;
        }

        public void SetIncomingDmNaxStreamAddress(string address)
        {
            if (IsTransmitter)
            {
                this.LogInformation("Cannot set incoming DM-NAX stream address on a transmitter device");
                return;
            }

            if (device.DmNaxRouting.DmNaxReceive == null)
            {
                this.LogInformation("Device is missing DM-NAX routing capabilities");
                return;
            }

            device.DmNaxRouting.DmNaxReceive.MulticastAddress.StringValue = address;
        }

        public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxDeviceJoinMap(joinStart);
            var customJoinMap = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);
            if (customJoinMap != null)
            {
                joinMap.SetCustomJoinData(customJoinMap);
            }
            bridge.AddJoinMap(Key, joinMap);

            trilist.SetBool(joinMap.DeviceOnline.JoinNumber, device.IsOnline);
            trilist.SetUshort(joinMap.VideoInput.JoinNumber, (ushort) device.Control.ActiveVideoSourceFeedback);
            trilist.SetString(joinMap.VideoInput.JoinNumber, device.Control.ActiveVideoSourceFeedback.ToString());
            trilist.SetUshort(joinMap.AudioInput.JoinNumber, (ushort) device.Control.ActiveAudioSourceFeedback);
            trilist.SetString(joinMap.AudioInput.JoinNumber, device.Control.ActiveAudioSourceFeedback.ToString());
            trilist.SetUshort(joinMap.NaxTxInput.JoinNumber, (ushort) device.Control.ActiveDmNaxAudioSourceFeedback);
            trilist.SetString(joinMap.DanteTxInput.JoinNumber, device.Control.ActiveDanteAudioSourceFeedback.ToString());
            trilist.SetUshort(joinMap.DanteTxInput.JoinNumber, (ushort) device.Control.ActiveDanteAudioSourceFeedback);
            trilist.SetString(joinMap.DanteTxInput.JoinNumber, device.Control.ActiveDanteAudioSourceFeedback.ToString());
            trilist.SetString(joinMap.MulticastVideoAddress.JoinNumber, device.Control.MulticastAddressFeedback.StringValue);

            device.OnlineStatusChange += (s, e) =>
            {
                trilist.SetBool(joinMap.DeviceOnline.JoinNumber, e.DeviceOnLine);
            };

            device.BaseEvent += (s, e) =>
            {
                bridge.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(device.BaseEvent));
                switch (e.EventId)
                {
                    case DMInputEventIds.NameFeedbackEventId:
                        trilist.SetString(joinMap.DeviceName.JoinNumber, device.Name);
                        break;
                    case DMInputEventIds.ActiveVideoSourceEventId:
                        trilist.SetUshort(joinMap.VideoInput.JoinNumber, (ushort) device.Control.ActiveVideoSourceFeedback);
                        trilist.SetString(joinMap.VideoInput.JoinNumber, device.Control.ActiveVideoSourceFeedback.ToString());
                        break;
                    case DMInputEventIds.ActiveAudioSourceEventId:
                        trilist.SetUshort(joinMap.AudioInput.JoinNumber, (ushort) device.Control.ActiveAudioSourceFeedback);
                        trilist.SetString(joinMap.AudioInput.JoinNumber, device.Control.ActiveAudioSourceFeedback.ToString());
                        break;
                    case DMInputEventIds.ActiveDmNaxAudioSourceFeedbackEventId:
                        trilist.SetUshort(joinMap.NaxTxInput.JoinNumber, (ushort) device.Control.ActiveDmNaxAudioSourceFeedback);
                        trilist.SetString(joinMap.NaxTxInput.JoinNumber, device.Control.ActiveDmNaxAudioSourceFeedback.ToString());
                        break;
                    case DMInputEventIds.ActiveDanteAudioSourceEventId:
                        trilist.SetUshort(joinMap.DanteTxInput.JoinNumber, (ushort) device.Control.ActiveDanteAudioSourceFeedback);
                        trilist.SetString(joinMap.DanteTxInput.JoinNumber, device.Control.ActiveDanteAudioSourceFeedback.ToString());
                        break;
                    case DMInputEventIds.MulticastAddressEventId:
                        trilist.SetString(joinMap.MulticastVideoAddress.JoinNumber, device.Control.MulticastAddressFeedback.StringValue);
                        break;
                }
            };

            if (device.DmNaxRouting is { } dmNax)
            {
                dmNax.DmNaxRoutingChange += (s, e) =>
                {
                    this.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(dmNax.DmNaxRoutingChange));
                    trilist.SetString(joinMap.NaxTxAddress.JoinNumber, dmNax.DmNaxTransmit.MulticastAddress.StringValue);
                    trilist.SetString(joinMap.NaxRxAddress.JoinNumber, dmNax.DmNaxReceive.MulticastAddress.StringValue);
                };
            }

            StreamUrlFeedback.LinkInputSig(trilist.StringInput[joinMap.StreamUrl.JoinNumber]);
            trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, SetIncomingStreamUrl);
            trilist.SetStringSigAction(joinMap.NaxRxAddress.JoinNumber, SetIncomingDmNaxStreamAddress);

            if (device.HdmiIn?[1] is { } hdmiIn1)
            {
                hdmiIn1.StreamChange += (s, e) =>
                {
                    this.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(hdmiIn1.StreamChange));
                    trilist.SetUshort(joinMap.Hdmi1Capability.JoinNumber, (ushort) hdmiIn1.HdcpCapabilityFeedback);
                    trilist.SetBool(joinMap.Hdmi1SyncDetected.JoinNumber, hdmiIn1.SyncDetectedFeedback.BoolValue);
                    trilist.SetString(joinMap.Hdmi1Name.JoinNumber, hdmiIn1.NameFeedback.StringValue);
                };

                trilist.SetUshort(joinMap.Hdmi1Capability.JoinNumber, (ushort) hdmiIn1.HdcpCapabilityFeedback);
                trilist.SetBool(joinMap.Hdmi1SyncDetected.JoinNumber, hdmiIn1.SyncDetectedFeedback.BoolValue);
                trilist.SetString(joinMap.Hdmi1Name.JoinNumber, hdmiIn1.NameFeedback.StringValue);
            }

            if (device.DmIn is { } dmIn)
            {
                dmIn.InputStreamChange += (s, e) =>
                {
                    this.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(dmIn.InputStreamChange));
                    trilist.SetUshort(joinMap.Hdmi1Capability.JoinNumber, (ushort) dmIn.HdcpCapabilityFeedback);
                    trilist.SetBool(joinMap.Hdmi1SyncDetected.JoinNumber, dmIn.SyncDetectedFeedback.BoolValue);
                    trilist.SetString(joinMap.Hdmi1Name.JoinNumber, dmIn.NameFeedback.StringValue);
                };

                trilist.SetUshort(joinMap.Hdmi1Capability.JoinNumber, (ushort) dmIn.HdcpCapabilityFeedback);
                trilist.SetBool(joinMap.Hdmi1SyncDetected.JoinNumber, dmIn.SyncDetectedFeedback.BoolValue);
                trilist.SetString(joinMap.Hdmi1Name.JoinNumber, dmIn.NameFeedback.StringValue);
            }

            if (device.HdmiIn?.Count >= 2 && device.HdmiIn[2] is { } hdmiIn2)
            {
                hdmiIn2.StreamChange += (s, e) =>
                {
                    this.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(hdmiIn2.StreamChange));
                    trilist.SetUshort(joinMap.Hdmi2Capability.JoinNumber, (ushort) hdmiIn2.HdcpCapabilityFeedback);
                    trilist.SetBool(joinMap.Hdmi2SyncDetected.JoinNumber, hdmiIn2.SyncDetectedFeedback.BoolValue);
                    trilist.SetString(joinMap.Hdmi2Name.JoinNumber, hdmiIn2.NameFeedback.StringValue);
                };

                trilist.SetUshort(joinMap.Hdmi2Capability.JoinNumber, (ushort) hdmiIn2.HdcpCapabilityFeedback);
                trilist.SetBool(joinMap.Hdmi2SyncDetected.JoinNumber, hdmiIn2.SyncDetectedFeedback.BoolValue);
                trilist.SetString(joinMap.Hdmi2Name.JoinNumber, hdmiIn2.NameFeedback.StringValue);
            }

            if (device is DmNvx38x nvx38x)
            {
                if (nvx38x.UsbcIn[1] is { } usbcIn1)
                {
                    usbcIn1.StreamChange += (s, e) =>
                    {
                        this.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(usbcIn1.StreamChange));
                        trilist.SetUshort(joinMap.Usbc1Capability.JoinNumber, (ushort) usbcIn1.HdcpCapabilityFeedback);
                        trilist.SetBool(joinMap.Usbc1SyncDetected.JoinNumber, usbcIn1.SyncDetectedFeedback.BoolValue);
                        trilist.SetString(joinMap.Usbc1Name.JoinNumber, usbcIn1.NameFeedback.StringValue);
                    };

                    trilist.SetUshort(joinMap.Usbc1Capability.JoinNumber, (ushort) usbcIn1.HdcpCapabilityFeedback);
                    trilist.SetBool(joinMap.Usbc1SyncDetected.JoinNumber, usbcIn1.SyncDetectedFeedback.BoolValue);
                    trilist.SetString(joinMap.Usbc1Name.JoinNumber, usbcIn1.NameFeedback.StringValue);
                }

                if (nvx38x.UsbcIn[2] is { } usbcIn2)
                {
                    usbcIn2.StreamChange += (s, e) =>
                    {
                        this.LogDebug("Received event {EventId} from:{Event}", e.EventId, nameof(usbcIn2.StreamChange));
                        trilist.SetUshort(joinMap.Usbc2Capability.JoinNumber, (ushort) usbcIn2.HdcpCapabilityFeedback);
                        trilist.SetBool(joinMap.Usbc2SyncDetected.JoinNumber, usbcIn2.SyncDetectedFeedback.BoolValue);
                        trilist.SetString(joinMap.Usbc2Name.JoinNumber, usbcIn2.NameFeedback.StringValue);
                    };

                    trilist.SetUshort(joinMap.Usbc2Capability.JoinNumber, (ushort) usbcIn2.HdcpCapabilityFeedback);
                    trilist.SetBool(joinMap.Usbc2SyncDetected.JoinNumber, usbcIn2.SyncDetectedFeedback.BoolValue);
                    trilist.SetString(joinMap.Usbc2Name.JoinNumber, usbcIn2.NameFeedback.StringValue);
                }
            }
        }

        /*
        private static void UpdateNetworkPortJoins(BasicTriList trilist, NvxDeviceJoinMap joinMap, INvxNetworkPortInformation networkPortInformation)
        {
            for (uint i = 0; i < Math.Min(joinMap.PortIndex.JoinSpan, networkPortInformation.NetworkPorts.Count); i++)
            {
                var port = networkPortInformation.NetworkPorts[(int)i];
                trilist.SetUshort(joinMap.PortIndex.JoinNumber + i, (ushort) port.DevicePortIndex);
                trilist.SetString(joinMap.PortName.JoinNumber + i, port.PortName);
                trilist.SetString(joinMap.PortDescription.JoinNumber + i, port.PortDescription);
                trilist.SetString(joinMap.PortVlanName.JoinNumber + i, port.VlanName);
                trilist.SetString(joinMap.PortIpManagementAddress.JoinNumber + i, port.IpManagementAddress);
                trilist.SetString(joinMap.PortSystemName.JoinNumber + i, port.SystemName);
                trilist.SetString(joinMap.PortSystemNameDescription.JoinNumber + i, port.SystemNameDescription);
            }
        }*/
    }
}
