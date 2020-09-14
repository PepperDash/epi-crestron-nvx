using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.DeviceExtensions;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Aggregates
{
    public abstract class NvxBaseDevice : CrestronGenericBridgeableBaseDevice, INvxDevice
    {
        protected readonly INvxDevice _device;
        protected readonly IBridgeAdvanced _bridge;

        protected NvxBaseDevice(INvxDevice device) : base(device.Key, device.Name, device.Hardware)
        {
            _device = device;
            _device.AddStreams();
            _device.AddNaxAudio();

            AddPreActivationAction(() =>
            {
                _device.RegisterForDeviceFeedback();

                Hardware.OnlineStatusChange += (@base, args) =>
                {
                    if (!args.DeviceOnLine)
                        return;

                    _device.SetDeviceDefaults();
                };
            });
        }

        public override bool CustomActivate()
        {
            Feedbacks.AddRange(_device.Feedbacks);

            Feedbacks
                .ToList()
                .ForEach(feedback => feedback.OutputChange += FeedbackOnOutputChange);

            return base.CustomActivate();
        }

        public static void PrintDevicesInfo()
        {
            var devices = DeviceManager.GetDevices()
                .OfType<INvxDevice>();

            foreach (var device in devices)
                device.PrintInfoToConsole();
        }

        private void FeedbackOnOutputChange(object sender, FeedbackEventArgs feedbackEventArgs)
        {
            var keyed = sender as IKeyed;
            if (keyed == null)
                return;

            if (sender is BoolFeedback)
                Debug.Console(1, this, "Received {0} Update : '{1}'", keyed.Key, feedbackEventArgs.BoolValue);

            if (sender is IntFeedback)
                Debug.Console(1, this, "Received {0} Update : '{1}'", keyed.Key, feedbackEventArgs.IntValue);

            if (sender is StringFeedback)
                Debug.Console(1, this, "Received {0} Update : '{1}'", keyed.Key, feedbackEventArgs.StringValue);
        }

        public new DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public int VirtualDeviceId
        {
            get { return _device.VirtualDeviceId; }
        }

        public DeviceConfig Config
        {
            get { return _device.Config; }
        }

        public BoolFeedback IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public StringFeedback DeviceName
        {
            get { return _device.DeviceName; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _device.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _device.VideoStreamStatus; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            _bridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        }
    }
}