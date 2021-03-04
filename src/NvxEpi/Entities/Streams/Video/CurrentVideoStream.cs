using System;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Routing;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Video
{
    public class CurrentVideoStream : VideoStream, ICurrentStream
    {
        public const string RouteNameKey = "CurrentVideoRoute";
        public const string RouteValueKey = "CurrentVideoRouteValue";
        private readonly IntFeedback _currentStreamId;
        private readonly StringFeedback _currentStreamName;
        private readonly CCriticalSection _lock = new CCriticalSection();
        private IStream _current;

        public CurrentVideoStream(INvxHardware device) : base(device)
        {
            _currentStreamId = IsTransmitter
                ? new IntFeedback(RouteValueKey, () => default( int ))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default( int ));

            _currentStreamName = IsTransmitter
                ? new StringFeedback(RouteNameKey, () => String.Empty)
                : new StringFeedback(RouteNameKey,
                    () => _current != null ? _current.VideoName.StringValue : NvxGlobalRouter.NoSourceText);

            Feedbacks.Add(_currentStreamId);
            Feedbacks.Add(_currentStreamName);

            Initialize();
        }

        public IntFeedback CurrentStreamId
        {
            get { return _currentStreamId; }
        }

        public StringFeedback CurrentStreamName
        {
            get { return _currentStreamName; }
        }

        public void UpdateCurrentRoute()
        {
            if (!IsOnline.BoolValue || IsTransmitter)
                return;

            try
            {
                _lock.Enter();
                _current = GetCurrentStream();

                CurrentStreamId.FireUpdate();
                CurrentStreamName.FireUpdate();
            }
            finally
            {
                _lock.Leave();
            }
        }

        private IStream GetCurrentStream()
        {
            try
            {
                if (!IsStreamingVideo.BoolValue)
                    return null;

                return DeviceManager
                    .AllDevices
                    .OfType<IStream>()
                    .Where(t => t.IsTransmitter && t.IsStreamingVideo.BoolValue)
                    .FirstOrDefault(
                        tx => tx.StreamUrl.StringValue.Equals(StreamUrl.StringValue, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Debug.Console(1,
                    this,
                    "Error getting current video route : {0}\r{1}\r{2}",
                    ex.Message,
                    ex.InnerException,
                    ex.StackTrace);

                throw;
            }
        }

        private void Initialize()
        {
            IsOnline.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            VideoStreamStatus.OutputChange += (sender, args) => UpdateCurrentRoute();
            IsStreamingVideo.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            StreamUrl.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
        }
    }
}