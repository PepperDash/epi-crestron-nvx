using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Output
{
    public class VideowallModeOutput : HdmiOutput, IVideowallMode
    {
        public VideowallModeOutput(INvx35XHardware device) : base(device)
        {
            Hardware = device.Hardware;

            VideowallMode = VideowallModeFeedback.GetFeedback(Hardware);
            Feedbacks.Add(VideowallMode);
        }

        public DmNvx35x Hardware { get; private set; }
        public IntFeedback VideowallMode { get; private set; }
    }
}