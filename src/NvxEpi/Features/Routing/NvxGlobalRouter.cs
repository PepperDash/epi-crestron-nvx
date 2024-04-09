using System;
using System.Collections.Generic;
using PepperDash.Core;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Services.TieLines;
using NvxEpi.Services.Utilities;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Devices;

#if SERIES4
using PepperDash.Essentials.AppServer.Messengers;
using PepperDash.Essentials.Core.DeviceTypeInterfaces;
#endif

namespace NvxEpi.Features.Routing
{
#if SERIES4
    public class NvxGlobalRouter : EssentialsDevice, IRoutingNumeric, IMatrixRouting
#else
    public class NvxGlobalRouter : EssentialsDevice, IRoutingNumeric
#endif
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

            AddPostActivationAction(BuildTieLines);

#if SERIES4
            AddPostActivationAction(BuildMatrixRouting);


            InputSlots = new Dictionary<string, IRoutingInputSlot>();
            OutputSlots = new Dictionary<string, IRoutingOutputSlot>();            

            //AddPostActivationAction(BuildMobileControlMessenger);
#endif
        }

        public static NvxGlobalRouter Instance { get { return _instance; } }

        private static void BuildTieLines()
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

#if SERIES4
        public Dictionary<string, IRoutingInputSlot> InputSlots { get; private set; }

        public Dictionary<string, IRoutingOutputSlot> OutputSlots { get; private set; }

        private void BuildMatrixRouting()
        {
            try
            {
                InputSlots = DeviceManager.AllDevices
                    .OfType<NvxBaseDevice>()
                    .Where(t => t.IsTransmitter)
                    .Select(t =>
                    {
                        return new NvxMatrixInput(t);
                    })
                    .Cast<IRoutingInputSlot>()
                    .ToDictionary(i => i.Key, i => i);

                var clearInput = new NvxMatrixClearInput();
                InputSlots.Add(clearInput.Key, clearInput);

                var transmitters = DeviceManager.AllDevices
                   .OfType<NvxBaseDevice>()
                   .Where(t =>
                   {
                       Debug.Console(0, this, $"{t.Key} is transmitter: {t.IsTransmitter}");
                       return !t.IsTransmitter;
                   }).ToList();

                Debug.Console(2, this, $"Receiver count: {transmitters.Count}");

                OutputSlots = transmitters.Select((t) =>
                {
                    Debug.Console(0, this, $"Getting NvxMatrixOutput for {t.Key}");

                    return new NvxMatrixOutput(t);
                }).Cast<IRoutingOutputSlot>().ToDictionary(t => t.Key, t => t);

            }
            catch (Exception ex)
            {
                Debug.LogMessage(ex, "Exception building MatrixRouting: {message}", this, ex.Message);
            }
        }

        private void BuildMobileControlMessenger()
        {
            var mc = DeviceManager.AllDevices.OfType<IMobileControl>().FirstOrDefault();

            if(mc == null)
            {
                Debug.Console(0, this, "Unable to find mobile control device");
                return;
            }

            var routingMessenger = new IMatrixRoutingMessenger($"{Key}-matrixRoutingMessenger", $"/device/{Key}", this);
            mc.AddDeviceMessenger(routingMessenger);
        }

        public void Route(string inputSlotKey, string outputSlotKey, eRoutingSignalType type)
        {
            if(!InputSlots.TryGetValue(inputSlotKey, out var inputSlot))
            {
                Debug.Console(0, this, "Unable to find input slot with key {0}", inputSlotKey);
                return;
            }

            if(!OutputSlots.TryGetValue(outputSlotKey, out var outputSlot))
            {
                Debug.Console(0, this, "Unable to find output slot with key {0}", outputSlotKey);
                return;
            }

            if(!(outputSlot is NvxMatrixOutput output))
            {
                Debug.LogMessage(Serilog.Events.LogEventLevel.Error, "Output with key {key} is not NvxMatrixOutput", this, outputSlotKey);
                return;
            }

            var outputDevice = output.Device;

            if (outputDevice == null)
            {
                Debug.Console(0, this, "Unable to get device to route");
                return;
            }

            if(type.Has(eRoutingSignalType.Video))
            {
               // using namespace to qualify type as `Route` is a static method
               Routing.PrimaryStreamRouter.Route(inputSlot.SlotNumber, outputDevice);                
            }

            if((type.Has(eRoutingSignalType.SecondaryAudio)
                || type.Has(eRoutingSignalType.Audio))
                && outputDevice is ISecondaryAudioStreamWithHardware audioOutput)
            {
                // using namespace to qualify type as `Route` is a static method
                Routing.SecondaryAudioRouter.Route(inputSlot.SlotNumber, audioOutput);
            }
        }
#endif
    }
}