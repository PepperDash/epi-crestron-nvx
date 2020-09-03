using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Models.Entities
{
    public class NvxHdmiOutput : IHdmiOutput
    {
        private readonly INvxDevice _device;

        public NvxHdmiOutput(INvxDevice device)
        {
            _device = device;
            
            Initialize();
        }

        private void Initialize()
        {
            if (_device.Hardware.HdmiOut == null)
                throw new NotSupportedException("Hdmi Output");

            HorizontalResolution = _device.Hardware.GetHorizontalResolutionFeedback();
            DisabledByHdcp = _device.Hardware.GetHdmiOutputDisabledFeedback();

            _device.Feedbacks.Add(HorizontalResolution);
            _device.Feedbacks.Add(DisabledByHdcp);
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

        public IntFeedback HorizontalResolution { get; private set; }
        public BoolFeedback DisabledByHdcp { get; private set; }

        public UsageTracking UsageTracker
        {
            get { return _device.UsageTracker; }
            set { _device.UsageTracker = value; }
        }
    }
}