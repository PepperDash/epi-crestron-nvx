using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.NaxAudio;
using NvxEpi.Entities.Routing;
using NvxEpi.Extensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public class CurrentNaxAudioStream : ICurrentNaxAudioStream
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly INaxAudioStream _stream;
        private INaxAudioTx _current;

        public const string RouteNameKey = "CurrentNaxAudioRoute";
        public const string RouteValueKey = "CurrentNaxAudioRouteValue";

        public CurrentNaxAudioStream(INaxAudioStream stream)
        {
            _stream = stream;
            Initialize();
        }

        private void Initialize()
        {
            CurrentNaxAudioStreamId = _stream.IsTransmitter
                ? new IntFeedback(RouteValueKey, () => default(int))
                : new IntFeedback(RouteValueKey, () => _current != null ? _current.DeviceId : default(int));

            CurrentNaxAudioStreamName = _stream.IsTransmitter
                ? new StringFeedback(RouteNameKey, () => String.Empty)
                : new StringFeedback(RouteNameKey, () => _current != null ? _current.AudioName.StringValue : NvxGlobalRouter.NoSourceText);

            IsOnline.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            IsStreamingNaxRx.OutputChange += (sender, args) => UpdateCurrentAudioRoute();
            NaxAudioRxAddress.OutputChange += (sender, args) => UpdateCurrentAudioRoute();

            Feedbacks.Add(CurrentNaxAudioStreamId);
            Feedbacks.Add(CurrentNaxAudioStreamName);
        }

        private void UpdateCurrentAudioRoute()
        {
            if (!IsOnline.BoolValue || IsTransmitter)
                return;

            try
            {
                _lock.Enter();
                _current = _stream.GetCurrentAudioRoute();

                CurrentNaxAudioStreamId.FireUpdate();
                CurrentNaxAudioStreamName.FireUpdate();
            }
            finally
            {
                _lock.Leave();
            }
        }

        public StringFeedback CurrentNaxAudioStreamName { get; private set; }
        public IntFeedback CurrentNaxAudioStreamId { get; private set; }

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

        public DmNvxBaseClass Hardware
        {
            get { return _stream.Hardware; }
        }

        public StringFeedback NaxAudioRxAddress
        {
            get { return _stream.NaxAudioRxAddress; }
        }

        public BoolFeedback IsStreamingNaxRx
        {
            get { return _stream.IsStreamingNaxRx; }
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

        public BoolFeedback IsOnline
        {
            get { return _stream.IsOnline; }
        }

        public StringFeedback VideoName
        {
            get { return _stream.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _stream.AudioName; }
        }

        public StringFeedback NaxAudioTxAddress
        {
            get { return _stream.NaxAudioTxAddress; }
        }

        public BoolFeedback IsStreamingNaxTx
        {
            get { return _stream.IsStreamingNaxTx; }
        }

        public StringFeedback CurrentNaxInput
        {
            get { return _stream.CurrentNaxInput; }
        }

        public IntFeedback CurrentNaxInputValue
        {
            get { return _stream.CurrentNaxInputValue; }
        }
    }
}