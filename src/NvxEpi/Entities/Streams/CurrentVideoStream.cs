using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Routing;
using NvxEpi.Extensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public class CurrentVideoStream : ICurrentStream
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly IStream _stream;
        private IStream _current;

        public const string RouteNameKey = "CurrentVideoRoute";
        public const string RouteValueKey = "CurrentVideoRouteValue";

        public CurrentVideoStream(IStream stream)
        {
            _stream = stream;
            Initialize();   
        }

        private void Initialize()
        {
            CurrentStreamId = _stream.IsTransmitter
                ? new IntFeedback(RouteValueKey, () => default(int))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default(int));

            CurrentStreamName = _stream.IsTransmitter
                ? new StringFeedback(RouteNameKey, () => String.Empty)
                : new StringFeedback(RouteNameKey, () => _current != null ? _current.VideoName.StringValue : NvxGlobalRouter.NoSourceText);

            IsOnline.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            VideoStreamStatus.OutputChange += (sender, args) => UpdateCurrentRoute();
            IsStreamingVideo.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            StreamUrl.OutputChange += (currentDevice, args) => UpdateCurrentRoute();

            Feedbacks.Add(CurrentStreamId);
            Feedbacks.Add(CurrentStreamName);        
        }

        public void UpdateCurrentRoute()
        {
            if (!_stream.IsOnline.BoolValue || IsTransmitter)
                return;

            try
            {
                _lock.Enter();
                _current = _stream.GetCurrentStreamRoute();

                CurrentStreamId.FireUpdate();
                CurrentStreamName.FireUpdate();   
            }
            finally
            {     
                _lock.Leave();
            }
        }

        public StringFeedback CurrentStreamName { get; private set; }
        public IntFeedback CurrentStreamId { get; private set; }

        public string Key
        {
            get { return _stream.Key; }
        }

        public string Name
        {
            get { return _stream.Name; }
        }

        public IntFeedback DeviceMode
        {
            get { return _stream.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _stream.IsTransmitter; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _stream.Hardware; }
        }

        public int DeviceId
        {
            get { return _stream.DeviceId; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _stream.MulticastAddress; }
        }

        public StringFeedback StreamUrl
        {
            get { return _stream.StreamUrl; }
        }

        public BoolFeedback IsStreamingVideo
        {
            get { return _stream.IsStreamingVideo; }
        }

        public StringFeedback VideoStreamStatus
        {
            get { return _stream.VideoStreamStatus; }
        }

        public BoolFeedback IsOnline
        {
            get { return _stream.IsOnline; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _stream.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _stream.OutputPorts; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _stream.Feedbacks; }
        }

        public StringFeedback VideoName
        {
            get { return _stream.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _stream.AudioName; }
        }
    }
}