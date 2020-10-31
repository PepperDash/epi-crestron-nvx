using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams.Audio
{
    public class SecondaryAudioStream : ISecondaryAudioStream
    {
        private readonly INvx35XHardware _device;

        public SecondaryAudioStream(INvx35XHardware device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            SecondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);
            SecondaryAudioStreamStatus = SecondaryAudioStatusFeedback.GetFeedback(Hardware);
            IsStreamingSecondaryAudio = IsStreamingSecondaryAudioFeedback.GetFeedback(Hardware);

            Feedbacks.Add(SecondaryAudioAddress);
            Feedbacks.Add(SecondaryAudioStreamStatus);
            Feedbacks.Add(IsStreamingSecondaryAudio);
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public string Name
        {
            get { return _device.Name; }
        }

        public int DeviceId
        {
            get { return _device.DeviceId; }
        }

        public StringFeedback SecondaryAudioAddress { get; private set; }
        public BoolFeedback IsStreamingSecondaryAudio { get; private set; }
        public StringFeedback SecondaryAudioStreamStatus { get; private set; }

        private const string _noRouteAddress = "0.0.0.0";

        public void SetSecondaryAudioAddress(string address)
        {
            if (IsTransmitter)
                return;

            if (String.IsNullOrEmpty(address))
                return;

            Debug.Console(1, this, "Setting Secondary Audio Address : '{0}'", address);

            Hardware.Control.AudioSource = DmNvxControl.eAudioSource.SecondaryStreamAudio;
            Hardware.SecondaryAudio.MulticastAddress.StringValue = address;
        }

        public void ClearSecondaryAudioStream()
        {
            if (IsTransmitter)
                return;

            Debug.Console(1, this, "Clearing Secondary Audio Stream");
            Hardware.SecondaryAudio.MulticastAddress.StringValue = _noRouteAddress;
        }

        DmNvxBaseClass INvxHardware.Hardware
        {
            get { return _device.Hardware; }
        }

        public DmNvx35x Hardware
        {
            get { return _device.Hardware; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public FeedbackCollection<Feedback> Feedbacks
        {
            get { return _device.Feedbacks; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }
    }
}