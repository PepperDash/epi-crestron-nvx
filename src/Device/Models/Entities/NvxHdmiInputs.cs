using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Entities
{
    public class NvxHdmiInputs : IHdmiInputs
    {
        private readonly INvxDevice _device;
        private readonly Dictionary<uint, IHdmiInput> _inputs = new Dictionary<uint, IHdmiInput>();
 
        public NvxHdmiInputs(INvxDevice device)
        {
            _device = device;

            Initialize();
        }

        private void Initialize()
        {
            if (_device.Hardware.HdmiIn == null)
                throw new NotSupportedException("Hdmi In");

            for (uint x = 1; x <= _device.Hardware.HdmiIn.Count; x++)
            {
                _inputs.Add(x, new NvxHdmiInput((int)x, _device));
            }
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

        public BoolFeedback IsStreamingVideo
        {
            get { return _device.IsStreamingVideo; }
        }

        public StringFeedback DeviceName
        {
            get { return _device.DeviceName; }
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

        public ReadOnlyDictionary<uint, IHdmiInput> HdmiInputs 
        {
            get { return new ReadOnlyDictionary<uint, IHdmiInput>(_inputs); }
        }

        public UsageTracking UsageTracker
        {
            get { return _device.UsageTracker; }
            set { _device.UsageTracker = value; }
        }
    }
}