using System;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Device.Entities.Config;
using NvxEpi.Device.Entities.InputSwitching;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.Services.Bridge;
using NvxEpi.Device.Services.Feedback;
using NvxEpi.Device.Services.InputPorts;
using NvxEpi.Device.Services.InputSwitching;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Entities.Hardware
{
    public class Nvx35xHardware : NvxBaseHardware, INvx35XHardware
    {
        public Nvx35xHardware(DeviceConfig config, DmNvx35x hardware, FeedbackCollection<Feedback> feedbacks, BoolFeedback isOnline)
            : base(config, hardware, feedbacks, isOnline)
        {
            Hardware = hardware;
        }

        public new DmNvx35x Hardware { get; private set; }
    }
}