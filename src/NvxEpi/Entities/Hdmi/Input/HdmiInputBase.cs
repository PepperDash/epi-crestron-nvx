using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Input
{
    public abstract class HdmiInputBase : IHdmiInput
    {
        private readonly INvxHardware _device;

        protected readonly Dictionary<uint, IntFeedback> _capability = new Dictionary<uint, IntFeedback>();
        protected readonly Dictionary<uint, BoolFeedback> _sync = new Dictionary<uint, BoolFeedback>();

        protected HdmiInputBase(INvxHardware device)
        {
            _device = device;
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public StringFeedback StreamUrl
        {
            get { return _device.StreamUrl; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _device.SecondaryAudioAddress; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public ReadOnlyDictionary<uint, IntFeedback> HdcpCapability
        {
            get { return new ReadOnlyDictionary<uint, IntFeedback>(_capability); }
        }

        public ReadOnlyDictionary<uint, BoolFeedback> SyncDetected
        {
            get { return new ReadOnlyDictionary<uint, BoolFeedback>(_sync); }
        }
    }
}