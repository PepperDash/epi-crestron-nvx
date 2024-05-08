using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NvxEpi.Services.Feedback
{
    public static class HdmiOutputResolutionFeedback
    {
        public const string Key = "HdmiOutOutputResolution";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {            
            if(device.HdmiOut == null)
            {
                return new StringFeedback(Key, () => string.Empty);
            }

            var feedback = new StringFeedback(Key, () => string.Format("{0}x{1}@{2}",
                device.HdmiOut.VideoAttributes.HorizontalResolutionFeedback.UShortValue,
                device.HdmiOut.VideoAttributes.VerticalResolutionFeedback.UShortValue,
                device.HdmiOut.VideoAttributes.FramesPerSecondFeedback.UShortValue));

            device.HdmiOut.VideoAttributes.AttributeChange += (o, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}
