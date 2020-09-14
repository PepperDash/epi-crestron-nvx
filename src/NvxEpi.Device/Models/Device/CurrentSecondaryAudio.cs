using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Models.Aggregates;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Device
{
    public class CurrentSecondaryAudio : IHasSecondaryAudioStreamRouting
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly INvxDevice _device;
        private readonly IHasSecondaryAudioStream _stream;
        private INvxDevice _current;

        public CurrentSecondaryAudio(INvxDevice device)
        {
            _device = device;
            _stream = new SecondaryAudioStream(device);
            Initialize();
        }

        private void Initialize()
        {
            CurrentSecondaryAudioRouteValue = _device.IsTransmitter.BoolValue
                ? new IntFeedback(CurrentAudioRouteFeedback.ValueKey, () => default(int))
                : new IntFeedback(CurrentAudioRouteFeedback.ValueKey, () => _current != null ? _current.VirtualDeviceId : default(int));

            CurrentSecondaryAudioRouteName = _device.IsTransmitter.BoolValue
                ? new StringFeedback(CurrentAudioRouteFeedback.NameKey, () => String.Empty)
                : new StringFeedback(CurrentAudioRouteFeedback.NameKey, () => _current != null ? _current.Name : NvxDeviceRouter.NoSourceText);

            _device.Feedbacks.AddRange(new Feedback[]
            {
                CurrentSecondaryAudioRouteValue,
                CurrentSecondaryAudioRouteName
            });

            _device.Hardware.BaseEvent += (@base, args) => // feedback.FireUpdate();
            {
                if (args.EventId != DMInputEventIds.ServerUrlEventId && args.EventId != DMInputEventIds.StartEventId &&
                    args.EventId != DMInputEventIds.StopEventId && args.EventId != DMInputEventIds.PauseEventId &&
                    args.EventId != DMInputEventIds.AudioSourceEventId &&
                    args.EventId != DMInputEventIds.ActiveAudioSourceEventId &&
                    args.EventId != DMInputEventIds.StatusEventId) return;

                UpdateCurrentAudioRoute();
            };

            _device.Hardware.SecondaryAudio.SecondaryAudioChange += (@base, args) => //feedback.FireUpdate();
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
            try
            {
                _lock.Enter();
                _stream.TryGetCurrentAudioRoute(out _current);

                CurrentSecondaryAudioRouteValue.FireUpdate();
                CurrentSecondaryAudioRouteName.FireUpdate();
            }
            finally
            {
                _lock.Leave();
            }
        }

        public IntFeedback CurrentSecondaryAudioRouteValue { get; private set; }
        public StringFeedback CurrentSecondaryAudioRouteName { get; private set; }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _stream.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _stream.SecondaryAudioStreamStatus; }
        }

        public StringFeedback SecondaryAudioMulticastAddress
        {
            get { return _stream.SecondaryAudioMulticastAddress; }
        }
    }
}