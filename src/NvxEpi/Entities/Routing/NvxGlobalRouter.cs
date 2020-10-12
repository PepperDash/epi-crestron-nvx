using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Services.TieLines;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Routing
{
    public class NvxGlobalRouter : EssentialsDevice
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

            DeviceManager.AddDevice(PrimaryStreamRouter);
            DeviceManager.AddDevice(SecondaryAudioRouter);
        }

        public static NvxGlobalRouter Instance { get { return _instance; } }

        public override bool CustomActivate()
        {
            Debug.Console(1, this, "Building tie lines...");
            var transmitters = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(t => t.IsTransmitter);

            TieLineConnector.AddTieLinesForTransmitters(transmitters);

            var receivers = DeviceManager
                .AllDevices
                .OfType<INvxDevice>()
                .Where(t => !t.IsTransmitter);

            TieLineConnector.AddTieLinesForReceivers(receivers);

            return base.CustomActivate();
        }
    }
}