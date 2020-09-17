using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Aggregates;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Device
{
    public class PrimaryVideoStream : IHasVideoStreamRouting
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private INvxDevice _current;

        private readonly INvxDevice _device;
        
        public PrimaryVideoStream(INvxDevice device)
        {
            _device = device;
            Initialize();
        }

        public IntFeedback CurrentVideoRouteValue { get; private set; }
        public StringFeedback CurrentVideoRouteName { get; private set; }

        private void Initialize()
        {
            CurrentVideoRouteValue = _device.IsTransmitter.BoolValue 
                ? new IntFeedback(CurrentVideoRouteFeedback.ValueKey, () => default(int)) 
                : new IntFeedback(CurrentVideoRouteFeedback.ValueKey, () => _current != null ? _current.VirtualDeviceId : default(int));

            CurrentVideoRouteName = _device.IsTransmitter.BoolValue
                ? new StringFeedback(CurrentVideoRouteFeedback.ValueKey, () => String.Empty)
                : new StringFeedback(CurrentVideoRouteFeedback.ValueKey, () => _current != null ? _current.Name : NvxDeviceRouter.NoSourceText);

            _device.Feedbacks.AddRange(new Feedback[]
            {
                CurrentVideoRouteValue,
                CurrentVideoRouteName
            });

            _device.Hardware.BaseEvent += (@base, args) =>
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