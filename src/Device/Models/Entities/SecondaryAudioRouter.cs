using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Models.Entities
{
    public class SecondarySecondaryAudioRouter : ISecondaryAudioStreamRouting
    {
        private readonly CCriticalSection _lock = new CCriticalSection();
        private readonly ISecondaryAudioStream _device;
        private INvxDevice _current;

        public SecondarySecondaryAudioRouter(ISecondaryAudioStream device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            CurrentAudioRouteValue = IsTransmitter.BoolValue
                ? new IntFeedback(CurrentAudioRouteFeedback.ValueKey, () => default(int))
                : new IntFeedback(CurrentAudioRouteFeedback.ValueKey, () => _current != null ? _current.VirtualDeviceId : default(int));

            CurrentAudioRouteName = IsTransmitter.BoolValue
                ? new StringFeedback(CurrentAudioRouteFeedback.NameKey, () => String.Empty)
                : new StringFeedback(CurrentAudioRouteFeedback.NameKey, () => _current != null ? _current.Name : NvxRouter.Instance.NoSourceText);

            Feedbacks.AddRange(new Feedback[]
            {
                CurrentAudioRouteValue,
                CurrentAudioRouteName
            });

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
            try
            {
                _lock.Enter();
                _device.TryGetCurrentAudioRoute(out _current);

                CurrentAudioRouteValue.FireUpdate();
                CurrentAudioRouteName.FireUpdate();
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

        public BoolFeedback IsStreamingVideo
        {
            get { return _device.IsStreamingVideo; }
        }

        public StringFeedback DeviceName
        {
            get { return _device.DeviceName; }
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

        public BoolFeedback IsStreamingAudio
        {
            get { return _device.IsStreamingAudio; }
        }

        public StringFeedback AudioStreamStatus
        {
            get { return _device.AudioStreamStatus; }
        }

        public StringFeedback AudioMulticastAddress
        {
            get { return _device.AudioMulticastAddress; }
        }

        public IntFeedback CurrentAudioRouteValue { get; private set; }
        public StringFeedback CurrentAudioRouteName { get; private set; }

        public void SetAudioAddress(string address)
        {
            if (String.IsNullOrEmpty(address) || IsTransmitter.BoolValue)
                return;

            Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public void StartAudioStream()
        {
            if (IsTransmitter.BoolValue)
                return;

            Hardware.SecondaryAudio.Start();
        }

        public void StopAudioStream()
        {
            if (IsTransmitter.BoolValue)
                return;

            Hardware.SecondaryAudio.Stop();
        }

        public UsageTracking UsageTracker
        {
            get { return _device.UsageTracker; }
            set { _device.UsageTracker = value; }
        }
    }
}