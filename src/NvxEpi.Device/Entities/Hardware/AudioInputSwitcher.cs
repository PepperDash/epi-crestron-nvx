using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Device.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Hardware
{
    public class AudioInputSwitcher : ICurrentAudioInput
    {
        private readonly INvxDevice _device;

        public AudioInputSwitcher(INvxDevice device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            CurrentAudioInput = AudioInputFeedback.GetFeedback(Hardware);
            CurrentAudioInputValue = AudioInputValueFeedback.GetFeedback(Hardware);
        }

        public string Key
        {
            get { return _device.Key; }
        }

        public RoutingPortCollection<RoutingInputPort> InputPorts
        {
            get { return _device.InputPorts; }
        }

        public RoutingPortCollection<RoutingOutputPort> OutputPorts
        {
            get { return _device.OutputPorts; }
        }

        public IntFeedback DeviceMode
        {
            get { return _device.DeviceMode; }
        }

        public bool IsTransmitter
        {
            get { return _device.IsTransmitter; }
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

        public StringFeedback MulticastAddress
        {
            get { return _device.MulticastAddress; }
        }

        public StringFeedback CurrentAudioInput { get; private set; }
        public IntFeedback CurrentAudioInputValue { get; private set; }

        public void HandleSwitch(object input, eRoutingSignalType type)
        {
            throw new System.NotImplementedException();
        }
    }
}