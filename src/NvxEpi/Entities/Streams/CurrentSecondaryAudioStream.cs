using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Entities.Routing;
using NvxEpi.Extensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public class CurrentSecondaryAudioStream : ICurrentSecondaryAudioStream
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly ISecondaryAudioStream _stream;
        private ISecondaryAudioStream _current;

        public const string RouteNameKey = "CurrentSecondaryAudioRoute";
        public const string RouteValueKey = "CurrentSecondaryAudioRouteValue";

        public CurrentSecondaryAudioStream(ISecondaryAudioStream stream)
        {
            _stream = stream;
            Initialize();
        }

        private void Initialize()
        {
            CurrentSecondaryAudioStreamId = _stream.IsTransmitter
                ? new IntFeedback(RouteValueKey, () => default(int))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default(int));

            CurrentSecondaryAudioStreamName = _stream.IsTransmitter
                ? new StringFeedback(RouteNameKey, () => String.Empty)
                : new StringFeedback(RouteNameKey, () => _current != null ? _current.AudioName.StringValue : NvxGlobalRouter.NoSourceText);

            IsOnline.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            IsStreamingSecondaryAudio.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            SecondaryAudioStreamStatus.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            SecondaryAudioAddress.OutputChange += (sender, args) => UpdateCurrentAudioRoute();

            Feedbacks.Add(CurrentSecondaryAudioStreamId);
            Feedbacks.Add(CurrentSecondaryAudioStreamName);
        }

        private void UpdateCurrentAudioRoute()
        {
            if (!IsOnline.BoolValue || IsTransmitter)
                return;

            try
            {
                _lock.Enter();   
                _current = _stream.GetCurrentAudioRoute();

                CurrentSecondaryAudioStreamId.FireUpdate();
                CurrentSecondaryAudioStreamName.FireUpdate();
            }
            finally
            {
                _lock.Leave();
            }
        }

        public IntFeedback DeviceMode
        {
            get { return _stream.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _stream.IsTransmitter; }
        }

        public string Key
        {
            get { return _stream.Key; }
        }

        public string Name
        {
            get { return _stream.Name; }
        }

        public int DeviceId
        {
            get { return _stream.DeviceId; }
        }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _stream.SecondaryAudioAddress; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _stream.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _stream.SecondaryAudioStreamStatus; }
        }

        public StringFeedback CurrentSecondaryAudioStreamName { get; private set; }
        public IntFeedback CurrentSecondaryAudioStreamId { get; private set; }

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

        public StringFeedback MulticastAddress
        {
            get { return _stream.MulticastAddress; }
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

        public StringFeedback StreamUrl
        {
            get { return _stream.StreamUrl; }
        }

        public DmNvxBaseClass Hardware
        {
            get { return _stream.Hardware; }
        }
    }
}