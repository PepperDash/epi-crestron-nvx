using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Features.Routing;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Streams.Video
{
    public class CurrentVideoStream : VideoStream, ICurrentStream
    {
        public const string RouteNameKey = "CurrentVideoRoute";
        public const string RouteValueKey = "CurrentVideoRouteValue";
        private readonly IntFeedback _currentStreamId;
        private readonly StringFeedback _currentStreamName;
        private readonly CCriticalSection _lock = new CCriticalSection();
        private IStream _current;

        private readonly IList<IStream> _transmitters = new List<IStream>(); 

        public CurrentVideoStream(INvxDeviceWithHardware device) : base(device)
        {
            _currentStreamId = IsTransmitter
                ? new IntFeedback(() => default( int ))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default( int ));

            _currentStreamName = IsTransmitter
                ? new StringFeedback(() => String.Empty)
                : new StringFeedback(RouteNameKey,
                    () => _current != null ? _current.Name : NvxGlobalRouter.NoSourceText);

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

                if (_current == null)
                {
                    Debug.Console(2, this, "Current stream address: {0} device ID: {1}", "0.0.0.0", 0);
                }
                else
                {
                    Debug.Console(2, this, "Current stream address: {0} device ID: {1}", _current.MulticastAddress, _current.DeviceId);
                }

                CurrentStreamId.FireUpdate();
                CurrentStreamName.FireUpdate();
            }
            catch (Exception ex)
            {
                Debug.Console(1,
                    this,
                    "Error getting current video route : {0}\r{1}\r{2}",
                    ex.Message,
                    ex.InnerException,
                    ex.StackTrace);
            }
            finally
            {
                _lock.Leave();
            }
        }

        private IStream GetCurrentStream()
        {
            if (string.IsNullOrEmpty(StreamUrl.StringValue) || MulticastAddress.StringValue.Equals("0.0.0.0"))
                return null;

            var result = _transmitters
                .Where(x => !string.IsNullOrEmpty(x.StreamUrl.StringValue))
                .FirstOrDefault(
                    x => x.StreamUrl.StringValue.Equals(StreamUrl.StringValue, StringComparison.OrdinalIgnoreCase));

            if (result != null)
            {
                return result;
            }

            result = DeviceManager
                .AllDevices
                .OfType<IStream>()
                .Where(t => t.IsTransmitter)
                .Where(x => !string.IsNullOrEmpty(x.StreamUrl.StringValue))
                .FirstOrDefault(
                    tx => tx.StreamUrl.StringValue.Equals(StreamUrl.StringValue, StringComparison.OrdinalIgnoreCase));

            if (result != null)
            {
                _transmitters.Add(result);
            }

            return result;
        }

        private void Initialize()
        {
            IsOnline.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            VideoStreamStatus.OutputChange += (sender, args) => UpdateCurrentRoute();
            IsStreamingVideo.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            MulticastAddress.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
            StreamUrl.OutputChange += (currentDevice, args) => UpdateCurrentRoute();
        }
    }
}