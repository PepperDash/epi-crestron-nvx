using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Device.Entities.Routing;
using NvxEpi.Extensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Streams
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
                : new StringFeedback(RouteNameKey, () => _current != null ? _current.Name : NvxDeviceRouter.NoSourceText);

            Hardware.BaseEvent += (@base, args) => // feedback.FireUpdate();
            {
                if (args.EventId != DMInputEventIds.ServerUrlEventId && args.EventId != DMInputEventIds.StartEventId &&
                    args.EventId != DMInputEventIds.StopEventId && args.EventId != DMInputEventIds.PauseEventId &&
                    args.EventId != DMInputEventIds.AudioSourceEventId &&
                    args.EventId != DMInputEventIds.ActiveAudioSourceEventId &&
                    args.EventId != DMInputEventIds.StatusEventId) return;

                UpdateCurrentAudioRoute();
            };

            Hardware.SecondaryAudio.SecondaryAudioChange += (@base, args) => //feedback.FireUpdate();
            {
                if (args.EventId != DMInputEventIds.MulticastAddressEventId &&
                    args.EventId != DMInputEventIds.StartEventId && args.EventId != DMInputEventIds.StopEventId &&
                    args.EventId != DMInputEventIds.PauseEventId && args.EventId != DMInputEventIds.AudioSourceEventId &&
                    args.EventId != DMInputEventIds.ActiveAudioSourceEventId &&
                    args.EventId != DMInputEventIds.StatusEventId) return;

                UpdateCurrentAudioRoute();
            };
        }

        private void UpdateCurrentAudioRoute()
        {
            if (!IsOnline.BoolValue)
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

        DmNvxBaseClass INvxHardware.Hardware
        {
            get { return _stream.Hardware; }
        }

        public DmNvx35x Hardware
        {
            get { return _stream.Hardware; }
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
    }
}