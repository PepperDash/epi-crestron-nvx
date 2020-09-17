using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Device.Models.Device;
using NvxEpi.Device.Services.DeviceExtensions;
using PepperDash.Core.JsonStandardObjects;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace NvxEpi.Device.Models.Aggregates
{
    public class Nvx35x : CrestronGenericBridgeableBaseDevice, INvxDevice, INvx35XHardware, IComPorts, IIROutputPorts
    {
        private readonly INvxDevice _device;

        public Nvx35x(DeviceConfig config, DmNvx35x hardware)
            : base(device)
        {

        }

        public CrestronCollection<ComPort> ComPorts { get { return _device.Hardware.ComPorts; } }
        public int NumberOfComPorts { get { return _device.Hardware.NumberOfComPorts; } }

        public CrestronCollection<IROutputPort> IROutputPorts { get { return _device.Hardware.IROutputPorts; } }
        public int NumberOfIROutputPorts { get { return _device.Hardware.NumberOfIROutputPorts; } }

        public Cec StreamCec { get { return _device.Hardware.HdmiOut.StreamCec; } }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            var action = inputSelector as Action<eRoutingSignalType>;
            if (action == null)
                throw new NullReferenceException("action");

            action(signalType);
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmiter
        {
            get { return _device.IsTransmiter; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            throw new NotImplementedException();
        }

        public new DmNvx35x Hardware { get; private set; }
        DmNvxBaseClass INvxHardware.Hardware { get; }
    }
}