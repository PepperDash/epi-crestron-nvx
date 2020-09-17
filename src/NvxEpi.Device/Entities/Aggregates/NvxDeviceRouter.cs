using NvxEpi.Device.Models.Routing;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Aggregates
{
    public class NvxDeviceRouter : EssentialsDevice
    {
        private static readonly NvxDeviceRouter _instance = new NvxDeviceRouter();

        public const string RouteOff = "$off";
        public const string InstanceKey = "NvxRouter";
        public const string NoSourceText = "No Source";

        public IRouting PrimaryStreamRouter { get; private set; }
        public IRouting SecondaryAudioRouter { get; private set; }
        public IRouting NaxAudioRouter { get; private set; }

        private NvxDeviceRouter()
            : base(InstanceKey)
        {
            PrimaryStreamRouter = new PrimaryStreamRouter(Key + "-PrimaryStream");
            SecondaryAudioRouter = new SecondaryAudioRouter(Key + "-SecondaryAudio");
            DeviceManager.AddDevice(this);
            DeviceManager.AddDevice(PrimaryStreamRouter);
            DeviceManager.AddDevice(SecondaryAudioRouter);
        }
    }
}