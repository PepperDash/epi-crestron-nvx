using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.NaxAudio;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Streams
{
    public class NaxStream : INaxAudioStream
    {
        private readonly INvxDevice _device;
        private readonly INaxAudioTx _tx;
        private readonly INaxAudioRx _rx;

        public NaxStream(INvxDevice device)
        {
            _device = device;
            _tx = new NaxTxStream(_device);
            _rx = new NaxRxStream(_device);

            Initialize();
        }

        private void Initialize()
        {
            CurrentNaxInput = NaxInputFeedback.GetFeedback(_device.Hardware);
            CurrentNaxInputValue = NaxInputValueFeedback.GetFeedback(_device.Hardware);

            _device.Feedbacks.Add(CurrentNaxInput);
            _device.Feedbacks.Add(CurrentNaxInputValue);

            _device.IsOnline.OutputChange += (sender, args) =>
            {
                if (!args.BoolValue)
                    return;

                Hardware.DmNaxRouting.SecondaryAudioMode = 
                    IsTransmitter 
                    ? DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic 
                    : DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
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

        public StringFeedback NaxAudioTxAddress
        {
            get { return _tx.NaxAudioTxAddress; }
        }

        public BoolFeedback IsStreamingNaxTx
        {
            get { return _tx.IsStreamingNaxTx; }
        }

        public StringFeedback NaxAudioRxAddress
        {
            get { return _rx.NaxAudioRxAddress; }
        }

        public BoolFeedback IsStreamingNaxRx
        {
            get { return _rx.IsStreamingNaxRx; }
        }

        public StringFeedback CurrentNaxInput { get; private set; }
        public IntFeedback CurrentNaxInputValue { get; private set; }

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