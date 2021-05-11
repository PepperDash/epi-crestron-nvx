using System;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Services.TieLines;
using NvxEpi.Services.Utilities;
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

        private NvxGlobalRouter()
            : base(InstanceKey)
        {
            PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
            SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");

            InputPorts = new RoutingPortCollection<RoutingInputPort>();
            OutputPorts = new RoutingPortCollection<RoutingOutputPort>();

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

            var receivers = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(t => !t.IsTransmitter)
                .ToList();

            TieLineConnector.AddTieLinesForReceivers(receivers);

            var audioTransmitters = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .ToList();

            TieLineConnector.AddTieLinesForAudioTransmitters(audioTransmitters);

            var audioReceivers = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .ToList();

            TieLineConnector.AddTieLinesForAudioReceivers(audioReceivers);

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
            throw new NotImplementedException("Execute Numeric Switch");
        }
    }
}