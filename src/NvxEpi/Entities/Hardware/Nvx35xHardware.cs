using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Entities.Hardware
{
    public class Nvx35xHardware : NvxBaseHardware, INvx35XHardware
    {
        private readonly StringFeedback _secondaryAudioAddress;

        public Nvx35xHardware(DeviceConfig config, DmNvx35x hardware, FeedbackCollection<Feedback> feedbacks, BoolFeedback isOnline)
            : base(config, hardware, feedbacks, isOnline)
        {
            Hardware = hardware;

            _secondaryAudioAddress = SecondaryAudioAddressFeedback.GetFeedback(Hardware);
        }

        public new DmNvx35x Hardware { get; private set; }

        public override StringFeedback SecondaryAudioAddress
        {
            get { return _secondaryAudioAddress; }
        }
    }
}