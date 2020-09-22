using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Routing
{
    public class NvxDeviceRouter : EssentialsDevice
    {
        private static readonly NvxDeviceRouter _instance = new NvxDeviceRouter();

        private const string _instanceKey = "$NvxRouter";
        public const string RouteOff = "$off";
        public const string NoSourceText = "No Source";

        public IRouting PrimaryStreamRouter { get; private set; }
        public IRouting SecondaryAudioRouter { get; private set; }

        private NvxDeviceRouter()
            : base(_instanceKey)
        {
            PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
            SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");

            DeviceManager.AddDevice(PrimaryStreamRouter);
            DeviceManager.AddDevice(SecondaryAudioRouter);
        }

        public static NvxDeviceRouter Instance { get { return _instance; } }
    }
}