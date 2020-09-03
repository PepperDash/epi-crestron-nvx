using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Entities
{
    public class AudioInputSwitcher : IAudioInputSwitcher
    {
        private readonly INvxDevice _device;

        public AudioInputSwitcher(INvxDevice device)
        {
            _device = device;
            Initialize();
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

        public StringFeedback AudioInputName { get; private set; }
        public IntFeedback AudioInputValue { get; private set; }

        private void Initialize()
        {
            AudioInputName = Hardware.GetAudioInputFeedback();
            AudioInputValue = Hardware.GetAudioInputValueFeedback();

            Feedbacks.AddRange(new Feedback[]
            {
                AudioInputName,
                AudioInputValue
            });
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public UsageTracking UsageTracker
        {
            get { return _device.UsageTracker; }
            set { _device.UsageTracker = value; }
        }
    }
}