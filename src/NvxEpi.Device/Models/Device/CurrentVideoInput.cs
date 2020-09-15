using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Device
{
    public class CurrentVideoInput : IHasCurrentVideoInput
    {
        private readonly INvxDevice _device;

        public CurrentVideoInput(INvxDevice device)
        {
            _device = device;

            Initialize();
        }

        public StringFeedback VideoInputName { get; private set; }
        public IntFeedback VideoInputValue { get; private set; }

        private void Initialize()
        {
            VideoInputName = _device.Hardware.GetVideoInputFeedback();
            VideoInputValue = _device.Hardware.GetVideoInputValueFeedback();

            _device.Feedbacks.AddRange(new Feedback[]
            {
                VideoInputName,
                VideoInputValue
            });
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
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
    }
}