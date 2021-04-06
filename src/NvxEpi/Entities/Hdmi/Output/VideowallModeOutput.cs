using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Services.Feedback;
using PepperDash.Essentials.Core;

namespace NvxEpi.Entities.Hdmi.Output
{
    public class VideowallModeOutput : HdmiOutput, IVideowallMode
    {
        private readonly INvx35XDeviceWithHardware _hardware;
        private readonly IntFeedback _videowallMode;

        public VideowallModeOutput(INvx35XDeviceWithHardware device) : base(device)
        {
            _hardware = device;

            _videowallMode = VideowallModeFeedback.GetFeedback(Hardware);
            _hardware.Feedbacks.Add(_videowallMode);
        }

        public new DmNvx35x Hardware
        {
            get { return _hardware.Hardware; }
        }

        public IntFeedback VideowallMode { get { return _videowallMode; } }
    }
}