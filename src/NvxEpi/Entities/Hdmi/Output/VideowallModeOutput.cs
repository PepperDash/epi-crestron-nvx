using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Output
{
    public class VideowallModeOutput : HdmiOutput, IVideowallMode
    {
        private readonly DmNvx35x _hardware;
        private readonly IntFeedback _videowallMode;

        public VideowallModeOutput(INvx35XHardware device) : base(device)
        {
            _hardware = device.Hardware;

            _videowallMode = VideowallModeFeedback.GetFeedback(Hardware);
            Feedbacks.Add(VideowallMode);
        }

        public new DmNvx35x Hardware
        {
            get { return _hardware; }
        }

        public IntFeedback VideowallMode
        {
            get { return _videowallMode; }
        }
    }
}