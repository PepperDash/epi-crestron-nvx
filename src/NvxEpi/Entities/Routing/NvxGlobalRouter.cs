using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Services.TieLines;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Routing
{
    public class NvxGlobalRouter : EssentialsDevice, IRoutingNumeric
    {
        private static readonly NvxGlobalRouter _instance = new NvxGlobalRouter();

        public const string InstanceKey = "$NvxRouter";
        public const string RouteOff = "$off";
        public const string NoSourceText = "No Source";

        public IRouting PrimaryStreamRouter { get; private set; }
        public IRouting SecondaryAudioRouter { get; private set; }

        private readonly Dictionary<int, INvxDevice> _transmitters = new Dictionary<int, INvxDevice>();
        private readonly Dictionary<int, INvxDevice> _receivers = new Dictionary<int, INvxDevice>(); 

        private NvxGlobalRouter()
            : base(InstanceKey)
        {
            PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
            SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");

            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

            InputPorts.AddRange(PrimaryStreamRouter.InputPorts);
            InputPorts.AddRange(SecondaryAudioRouter.InputPorts);

            OutputPorts.AddRange(PrimaryStreamRouter.OutputPorts);
            OutputPorts.AddRange(SecondaryAudioRouter.OutputPorts);

            DeviceManager.AddDevice(PrimaryStreamRouter);
            DeviceManager.AddDevice(SecondaryAudioRouter);
        }

        public static NvxGlobalRouter Instance { get { return _instance; } }

        public override bool CustomActivate()
        {
            var transmitters = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(t => t.IsTransmitter)
                .ToList();

            TieLineConnector.AddTieLinesForTransmitters(transmitters);
            foreach (var transmitter in transmitters)
                _transmitters.Add(transmitter.DeviceId, transmitter);

            var receivers = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(t => !t.IsTransmitter)
                .ToList();

            TieLineConnector.AddTieLinesForReceivers(receivers);
            foreach (var receiver in receivers)
                _receivers.Add(receiver.DeviceId, receiver);

            return base.CustomActivate();
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts { get; private set; }
        public RoutingPortCollection<RoutingOutputPort> OutputPorts { get; private set; }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            if (signalType.Has(eRoutingSignalType.Video))
                PrimaryStreamRouter.ExecuteSwitch(inputSelector, outputSelector, signalType);

            if (signalType.Has(eRoutingSignalType.Audio))
                SecondaryAudioRouter.ExecuteSwitch(inputSelector, outputSelector, signalType);
        }

        public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
        {
            INvxDevice rx;
            if (!_receivers.TryGetValue(output, out rx))
                return;

            INvxDevice tx;
            _transmitters.TryGetValue(input, out tx);

            if (type.Has(eRoutingSignalType.Video))
                PrimaryStreamRouter.ExecuteSwitch(tx, rx, type);

            if (type.Has(eRoutingSignalType.Audio))
                SecondaryAudioRouter.ExecuteSwitch(tx, rx, type);
        }
    }
}