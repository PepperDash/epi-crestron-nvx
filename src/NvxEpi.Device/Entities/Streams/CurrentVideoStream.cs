using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Routing;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Streams
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
                : new StringFeedback(RouteNameKey, () => _current != null ? _current.Name : NvxDeviceRouter.NoSourceText);

            _stream.Hardware.BaseEvent += (@base, args) =>
            {
                if (args.EventId != DMInputEventIds.ServerUrlEventId && args.EventId != DMInputEventIds.StartEventId &&
                    args.EventId != DMInputEventIds.StopEventId && args.EventId != DMInputEventIds.PauseEventId &&
                    args.EventId != DMInputEventIds.VideoSourceEventId)
                    return;

                UpdateCurrentRoute();
            };

            _stream.Hardware.OnlineStatusChange += (currentDevice, args) => UpdateCurrentRoute();
        }

        public void UpdateCurrentRoute()
        { 
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
    }
}