using System.Collections.Generic;
using System.Linq;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi
{
    public class NvxMockDevice : EssentialsDevice, INvxDevice, IHasFeedback, IRoutingWithFeedback
    {
        private readonly List<RouteSwitchDescriptor> currentRoutes = new();

        private string streamUrl;
        private string dmNaxTxAddress;
        private string dmNaxRxAddress;
        private string multicastAddress;
        private readonly bool syncDetected;

        public NvxMockDevice(
            string key, string name, string streamUrl, string multicastAddress, string dmNaxTxAddress, string dmNaxRxAddress, int deviceId, bool isTransmitter = false) : base(key, name)
        {
            IsTransmitter = isTransmitter;
            DeviceId = deviceId;

            this.streamUrl = streamUrl;
            this.dmNaxTxAddress = dmNaxTxAddress;
            this.dmNaxRxAddress = dmNaxRxAddress;
            this.multicastAddress = multicastAddress;

            StreamUrlFeedback = new("StreamUrl", () => this.streamUrl);
            IsStreamingFeedback = new("IsStreaming", () => !string.IsNullOrEmpty(this.streamUrl));
            StreamStatusFeedback = new("StreamStatus", () => string.IsNullOrEmpty(this.streamUrl) ? "No Stream" : "Streaming");

            DmNaxTxAddressFeedback = new("DmNaxTxAddress", () => this.dmNaxTxAddress);
            IsTransmittingDmNaxFeedback = new("IsTransmittingDmNax", () => !string.IsNullOrEmpty(this.dmNaxTxAddress));
            DmNaxTransmitStatusFeedback = new("DmNaxTransmitStatus", () => string.IsNullOrEmpty(this.dmNaxTxAddress) ? "No Stream" : "Streaming");

            DmNaxRxAddressFeedback = new("DmNaxRxAddress", () => this.dmNaxRxAddress);
            IsReceivingDmNaxFeedback = new("IsReceivingDmNax", () => !string.IsNullOrEmpty(this.dmNaxRxAddress));
            DmNaxReceiveStatusFeedback = new("DmNaxReceiveStatus", () => string.IsNullOrEmpty(this.dmNaxRxAddress) ? "No Stream" : "Streaming");

            MulticastAddressFeedback = new("MulticastAddress", () => this.multicastAddress);

            Feedbacks = new()
            {
                StreamUrlFeedback,
                IsStreamingFeedback,
                StreamStatusFeedback,
                DmNaxTxAddressFeedback,
                IsTransmittingDmNaxFeedback,
                DmNaxTransmitStatusFeedback,
                DmNaxRxAddressFeedback,
                IsReceivingDmNaxFeedback,
                DmNaxReceiveStatusFeedback,
                MulticastAddressFeedback,
            };

            if (IsTransmitter)
            {
                this.LogInformation("Creating mock transmitter device. Name: {1}, StreamUrl: {2}, DmNaxTxAddress: {3}", name, streamUrl, dmNaxTxAddress);

                var input = new RoutingInputPort(NvxDevice.Hdmi1RoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, null, this);
                InputPorts = [input];

                var streamOutput = new RoutingOutputPort(NvxDevice.StreamRoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, null, this);
                var dmNaxOutput = new RoutingOutputPort(NvxDevice.DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, null, this);
                OutputPorts = [streamOutput, dmNaxOutput];

                currentRoutes.Add(new RouteSwitchDescriptor(streamOutput, input));
                currentRoutes.Add(new RouteSwitchDescriptor(dmNaxOutput, input));
            }
            else
            {
                this.LogInformation("Creating mock receiver device. Name: {1}", name);

                var streamInput = new RoutingInputPort(NvxDevice.StreamRoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, null, this);
                var dmNaxInput = new RoutingInputPort(NvxDevice.DmNaxRoutingPortKey, eRoutingSignalType.Audio, eRoutingPortConnectionType.Streaming, null, this);
                InputPorts = [streamInput, dmNaxInput];

                var output = new RoutingOutputPort(NvxDevice.Hdmi1RoutingPortKey, eRoutingSignalType.AudioVideo, eRoutingPortConnectionType.Streaming, null, this);
                OutputPorts = [output];

                currentRoutes.Add(new RouteSwitchDescriptor(output, streamInput));
                currentRoutes.Add(new RouteSwitchDescriptor(output, dmNaxInput));
            }

            SyncDetected = new BoolFeedback("SyncDetected", () => syncDetected);

            foreach (var fb in Feedbacks)
            {
                fb.FireUpdate();
            }

            if (IsTransmitter)
            {
                this.LogInformation("Mock transmitter device created. Name: {1}, StreamUrl: {2}, DmNaxTxAddress: {3}", name, streamUrl, dmNaxTxAddress);
                NvxRouter.RegisterTransmitter(this);
            }
            else
            {
                this.LogInformation("Mock receiver device created. Name: {1}", name);
                NvxRouter.RegisterReceiver(this);
            }
        }

        public bool IsTransmitter { get; }

        public StringFeedback StreamUrlFeedback { get; }
        public BoolFeedback IsStreamingFeedback { get; }
        public StringFeedback StreamStatusFeedback { get; }
        public StringFeedback DmNaxTxAddressFeedback { get; }
        public BoolFeedback IsTransmittingDmNaxFeedback { get; }
        public StringFeedback DmNaxTransmitStatusFeedback { get; }
        public StringFeedback DmNaxRxAddressFeedback { get; }
        public BoolFeedback IsReceivingDmNaxFeedback { get; }
        public StringFeedback DmNaxReceiveStatusFeedback { get; }
        public StringFeedback MulticastAddressFeedback { get; }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; }

        public int DeviceId { get; }

        public FeedbackCollection<Feedback> Feedbacks { get; }

        public List<RouteSwitchDescriptor> CurrentRoutes => currentRoutes.ToList();

        public event RouteChangedEventHandler? RouteChanged;

        public override bool CustomActivate()
        {
            foreach (var route in currentRoutes)
            {
                RouteChanged?.Invoke(this, route);
            }

            return base.CustomActivate();
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {

        }

        public void SetIncomingStreamUrl(string streamUrl)
        {
            if (IsTransmitter)
            {
                this.LogWarning("Received incoming stream url for transmitter device, ignoring. Url: {0}", streamUrl);
                return;
            }

            this.streamUrl = streamUrl;
            StreamUrlFeedback.FireUpdate();
        }

        public void SetIncomingDmNaxStreamAddress(string address)
        {
            if (IsTransmitter)
            {
                this.LogWarning("Received incoming DM-NAX stream address for transmitter device, ignoring. Address: {0}", address);
                return;
            }

            dmNaxRxAddress = address;
            DmNaxRxAddressFeedback.FireUpdate();
        }

        public BoolFeedback SyncDetected { get; }
    }
}
