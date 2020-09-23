using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Device.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Entities.Streams
{
    public class SecondaryAudioStream : ISecondaryAudioStream
    {
        private readonly INvx35XHardware _device;

        public SecondaryAudioStream(INvx35XHardware device, BoolFeedback isOnline)
        {
            _device = device;
            IsOnline = isOnline;
            Initialize();
        }

        private void Initialize()
        {
            SecondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);
            SecondaryAudioStreamStatus = SecondaryAudioStatusFeedback.GetFeedback(Hardware);
            IsStreamingSecondaryAudio = IsStreamingSecondaryAudioFeedback.GetFeedback(Hardware);
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

        DmNvxBaseClass INvxHardware.Hardware
        {
            get { return _device.Hardware; }
        }

        public DmNvx35x Hardware
        {
            get { return _device.Hardware; }
        }

        public BoolFeedback IsOnline { get; private set; }
    }
}