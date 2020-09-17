using System;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Services.Config;
using NvxEpi.Device.Services.DeviceExtensions;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Models.Aggregates
{
    public abstract class NvxDevice : CrestronGenericBridgeableBaseDevice, INvxDevice
    {
        private readonly RoutingPortCollection<RoutingInputPort> _inputs = new RoutingPortCollection<RoutingInputPort>();
        private readonly RoutingPortCollection<RoutingOutputPort> _outputs = new RoutingPortCollection<RoutingOutputPort>(); 

        protected NvxDevice(DeviceConfig config, DmNvxBaseClass hardware)
            : base(config.Key, config.Name, hardware)
        {
            Key = config.Key;
            Name = config.Name;
            Hardware = hardware;

            DeviceId = deviceId;
            IsTransmiter = isTransmiter;
            
            hardware.OnlineStatusChange += (device, args) =>
            {
                if (args.DeviceOnLine)
                    InitializeDevice();
            };

            SetupFeedbacks();
            AddRoutingPorts();
        }

        private void InitializeDevice()
        {
            Hardware.Control.Name.StringValue = Name.Replace(' ', '-');
            if (IsTransmiter)
                Hardware.Control.DeviceMode = eDeviceMode.Transmitter;
        }

        private void SetupFeedbacks()
        {
            DeviceMode = Feedbacks.DeviceMode.GetFeedback(Hardware);
            MulticastAddress = Feedbacks.MulticastAddress.GetFeedback(Hardware);
        }

        private void AddRoutingPorts()
        {
            throw new NotImplementedException();
        }

        public string Key { get; private set; }
        public string Name { get; private set; }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get { return _inputs; } }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get { return _outputs; } }

        public bool IsTransmiter { get; private set; }
        public int DeviceId { get; private set; }

        public IntFeedback DeviceMode { get; private set; } 
        public DmNvxBaseClass Hardware { get; private set; } 
        public StringFeedback MulticastAddress { get; private set; }
    }
}