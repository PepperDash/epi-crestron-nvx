using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models.Entities;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class HorizontalResolutionFeedback
    {
        public static readonly string Key = "HorizontalResolution";
        public static IntFeedback GetHorizontalResolutionFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(Key, () => device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue);

            device.HdmiOut.VideoAttributes.AttributeChange += (sender, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}