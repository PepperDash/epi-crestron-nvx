﻿using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Routing;
using NvxEpi.Extensions;
using PepperDash.Core;
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

            _stream.IsOnline.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            _stream.IsStreamingVideo.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            _stream.StreamUrl.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
        }

        public void UpdateCurrentRoute()
        {
            if (!_stream.IsOnline.BoolValue)
                return;

            try
            {
                _lock.Enter();
                Debug.Console(1, this, "Updating current stream...");
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
    }
}