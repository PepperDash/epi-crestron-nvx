using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Entities.Hardware
{
    public class NvxE3XHardware : NvxBaseHardware, INvxE3XHardware
    {
        public NvxE3XHardware(DeviceConfig config, DmNvxE3x hardware, FeedbackCollection<Feedback> feedbacks, BoolFeedback isOnline)
            : base(config, hardware, feedbacks, isOnline)
        {
            Hardware = hardware;
        }

        public new DmNvxE3x Hardware { get; private set; }
    }
}