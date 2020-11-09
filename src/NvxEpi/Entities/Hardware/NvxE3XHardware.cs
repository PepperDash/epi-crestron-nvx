using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Entities.Hardware
{
    public class NvxE3XHardware : NvxBaseHardware, INvxE3XHardware
    {
        private readonly StringFeedback _secondaryAudioAddress;

        public NvxE3XHardware(DeviceConfig config, DmNvxE3x hardware, FeedbackCollection<Feedback> feedbacks, BoolFeedback isOnline)
            : base(config, hardware, feedbacks, isOnline)
        {
            Hardware = hardware;
            IsTransmitter = true;

            _secondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);
        }

        public new DmNvxE3x Hardware { get; private set; }

        public override StringFeedback SecondaryAudioAddress
        {
            get { return _secondaryAudioAddress; }
        }
    }
}