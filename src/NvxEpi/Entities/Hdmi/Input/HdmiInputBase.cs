using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Input
{
    public abstract class HdmiInputBase : IHdmiInput
    {
        protected readonly Dictionary<uint, IntFeedback> _capability = new Dictionary<uint, IntFeedback>();
        protected readonly Dictionary<uint, BoolFeedback> _sync = new Dictionary<uint, BoolFeedback>();

        private readonly INvxDeviceWithHardware _device;

        protected HdmiInputBase(INvxDeviceWithHardware device)
        {
            _device = device;
        }

        public StringFeedback AudioSourceName
        {
            get { return _device.AudioSourceName; }
        }

        public StringFeedback AudioDestinationName
        {
            get { return _device.AudioDestinationName; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
        {
            get { return new ReadOnlyDictionary<uint, IntFeedback>(_capability); }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
        {
            get { return new ReadOnlyDictionary<uint, BoolFeedback>(_sync); }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }
    }
}