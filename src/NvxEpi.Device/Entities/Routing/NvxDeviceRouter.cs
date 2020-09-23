using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Routing
{
    public class NvxDeviceRouter : EssentialsDevice
    {
        private static readonly NvxDeviceRouter _instance = new NvxDeviceRouter();

        public const string InstanceKey = "$NvxRouter";
        public const string RouteOff = "$off";
        public const string NoSourceText = "No Source";

        public PrimaryStreamRouter PrimaryStreamRouter { get; private set; }
        public SecondaryAudioRouter SecondaryAudioRouter { get; private set; }

        private NvxDeviceRouter()
            : base(InstanceKey)
        {
            PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
            SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");

            DeviceManager.AddDevice(PrimaryStreamRouter);
            DeviceManager.AddDevice(SecondaryAudioRouter);
        }

        public static NvxDeviceRouter Instance { get { return _instance; } }
    }
}