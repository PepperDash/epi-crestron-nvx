using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Routing
{
    public class NvxGlobalRouter : EssentialsDevice
    {
        private static readonly NvxGlobalRouter _instance = new NvxGlobalRouter();

        public const string InstanceKey = "$NvxRouter";
        public const string RouteOff = "$off";
        public const string NoSourceText = "No Source";

        public PrimaryStreamRouter PrimaryStreamRouter { get; private set; }
        public SecondaryAudioRouter SecondaryAudioRouter { get; private set; }

        private NvxGlobalRouter()
            : base(InstanceKey)
        {
            PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
            SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");

            DeviceManager.AddDevice(PrimaryStreamRouter);
            DeviceManager.AddDevice(SecondaryAudioRouter);
        }

        public static NvxGlobalRouter Instance { get { return _instance; } }
    }
}