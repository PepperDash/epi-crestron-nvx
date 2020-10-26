using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.NaxAudio;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public class NaxTxStream : INaxAudioTx
    {
        private readonly INvxDevice _device;

        public NaxTxStream(INvxDevice device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            NaxAudioTxAddress = NaxTxAddressFeedback.GetFeedback(_device.Hardware);
            IsStreamingNaxTx = IsStreamingNaxTxAudioFeedback.GetFeedback(_device.Hardware);

            _device.Feedbacks.AddRange(new Feedback[]
            {
                NaxTxStatusFeedback.GetFeedback(_device.Hardware),
                NaxAudioTxAddress,
                IsStreamingNaxTx
            });

            _device.IsOnline.OutputChange += (sender, args) =>
            {
                if (!args.BoolValue)
                    return;

                _device.Hardware.DmNaxRouting.DmNaxTransmit.EnableAutomaticInitiation();
            };
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

        public DmNvxBaseClass Hardware
        {
            get { return _device.Hardware; }
        }

        public StringFeedback NaxAudioTxAddress { get; private set; }
        public BoolFeedback IsStreamingNaxTx { get; private set; }

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

        public BoolFeedback IsOnline
        {
            get { return _device.IsOnline; }
        }

        public StringFeedback VideoName
        {
            get { return _device.VideoName; }
        }

        public StringFeedback AudioName
        {
            get { return _device.AudioName; }
        }
    }
}