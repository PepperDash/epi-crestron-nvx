using System;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class VideoAspectRatioModeFeedback
    {
        public const string Key = "VideoAspectRatioMode";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiOut == null)
                return new IntFeedback(() => default (int));

            var feedback = new IntFeedback(Key, () => (int) device.HdmiOut.VideoAttributes.AspectRatioModeFeedback);
            device.HdmiOut.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();
            device.OnlineStatusChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class VideoAspectRatioModeFeedbackName
    {
        public const string Key = "VideoAspectRatioModeName";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiOut == null)
                return new StringFeedback(() => string.Empty);

            var feedback = new StringFeedback(Key,
                () => device.HdmiOut.VideoAttributes.AspectRatioModeFeedback.ToString());

            device.HdmiOut.VideoAttributes.AttributeChange += (stream, args) => feedback.FireUpdate();
            device.HdmiOut.StreamChange += (stream, args) => feedback.FireUpdate();
            device.OnlineStatusChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }
}