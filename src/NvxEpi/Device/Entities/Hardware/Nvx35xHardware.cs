using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Device.Entities.Streams;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Entities.Hardware
{
    public class Nvx35xHardware : NvxBaseHardware, ISecondaryAudioStream
    {
        private readonly ISecondaryAudioStream _secondaryAudioStream;

        public Nvx35xHardware(DeviceConfig config, DmNvx35x hardware, FeedbackCollection<Feedback> feedbacks, BoolFeedback isOnline)
            : base(config, hardware, feedbacks, isOnline)
        {
            Hardware = hardware;
            _secondaryAudioStream = new SecondaryAudioStream(this, isOnline);
        }

        public new DmNvx35x Hardware { get; private set; }

        public StringFeedback SecondaryAudioAddress
        {
            get { return _secondaryAudioStream.SecondaryAudioAddress; }
        }

        public BoolFeedback IsStreamingSecondaryAudio
        {
            get { return _secondaryAudioStream.IsStreamingSecondaryAudio; }
        }

        public StringFeedback SecondaryAudioStreamStatus
        {
            get { return _secondaryAudioStream.SecondaryAudioStreamStatus; }
        }
    }
}