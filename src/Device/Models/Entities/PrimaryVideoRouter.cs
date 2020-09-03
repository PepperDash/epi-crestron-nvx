using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Entities
{
    public class PrimaryVideoRouter : IVideoStreamRouting
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private INvxDevice _current;

        private readonly INvxDevice _device;
        
        public PrimaryVideoRouter(INvxDevice device)
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

        public IntFeedback CurrentVideoRouteValue { get; private set; }
        public StringFeedback CurrentVideoRouteName { get; private set; }


        private void Initialize()
        {
            CurrentVideoRouteValue = IsTransmitter.BoolValue 
                ? new IntFeedback(CurrentVideoRouteFeedback.ValueKey, () => default(int)) 
                : new IntFeedback(CurrentVideoRouteFeedback.ValueKey, () => _current != null ? _current.VirtualDeviceId : default(int));

            CurrentVideoRouteName = IsTransmitter.BoolValue
                ? new StringFeedback(CurrentVideoRouteFeedback.ValueKey, () => String.Empty)
                : new StringFeedback(CurrentVideoRouteFeedback.ValueKey, () => _current != null ? _current.Name : NvxRouter.Instance.NoSourceText);

            Feedbacks.AddRange(new Feedback[]
            {
                CurrentVideoRouteValue,
                CurrentVideoRouteName
            });

            Hardware.BaseEvent += (@base, args) =>
            {
                if (args.EventId != DMInputEventIds.ServerUrlEventId && args.EventId != DMInputEventIds.StartEventId &&
                    args.EventId != DMInputEventIds.StopEventId && args.EventId != DMInputEventIds.PauseEventId &&
                    args.EventId != DMInputEventIds.VideoSourceEventId)
                    return;

                UpdateCurrentRoute();
            };
        }

        private void UpdateCurrentRoute()
        { 
            try
            {
                _lock.Enter();
                _device.TryGetCurrentVideoRoute(out _current);

                CurrentVideoRouteName.FireUpdate();
                CurrentVideoRouteValue.FireUpdate();   
            }
            finally
            {     
                _lock.Leave();
            }
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