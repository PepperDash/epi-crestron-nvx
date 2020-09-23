using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Device.Entities.Config;
using NvxEpi.Device.Services.Bridge;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Entities.Aggregates
{
    public abstract class NvxBaseDevice : CrestronGenericBridgeableBaseDevice, INvxHardware, IRoutingInputsOutputs
    {
        private readonly RoutingPortCollection<RoutingInputPort> _inputs = new RoutingPortCollection<RoutingInputPort>();
        private readonly RoutingPortCollection<RoutingOutputPort> _outputs = new RoutingPortCollection<RoutingOutputPort>();

        protected NvxBaseDevice(DeviceConfig config, DmNvxBaseClass hardware)
            : base(config.Key, config.Name, hardware)
        {
            Key = config.Key;
            Name = config.Name;
            Hardware = hardware;
            var props = NvxDeviceProperties.FromDeviceConfig(config);

            hardware.OnlineStatusChange += (device, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                Hardware.Control.Name.StringValue = Name.Replace(' ', '-');

                if (IsTransmitter)
                    Hardware.SetTxDefaults(props);
                else
                    Hardware.SetRxDefaults(props);

                Hardware.SetDefaultInputs(props);
            };
        }

        public override bool CustomActivate()
        {
            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
            DeviceDebug.RegisterForRoutingInputPortFeedback(this);
            DeviceDebug.RegisterForRoutingOutputFeedback(this);

            foreach (var routingInputPort in InputPorts)
            {
                Debug.Console(1, this, "Routing Input Port : {0}", routingInputPort.Key);
            }

            foreach (var routingOutputPort in OutputPorts)
            {
                Debug.Console(1, this, "Routing Output Port : {0}", routingOutputPort.Key);
            }

            return base.CustomActivate();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get { return _inputs; } }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get { return _outputs; } }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var deviceBridge = new NvxDeviceBridge(this);
            deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        }

        public abstract IntFeedback DeviceMode { get; }
        public abstract bool IsTransmitter { get; }
        public abstract int DeviceId { get; }
        public new DmNvxBaseClass Hardware { get; private set; }
    }
}