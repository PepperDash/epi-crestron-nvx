using System;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class Hdmi1HdcpCapabilityFeedback
    {
        public const string Key = "Hdmi1HdcpCapability";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                return new StringFeedback(() => string.Empty);

            var feedback = new StringFeedback(Key,
                () => device.HdmiIn[1].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class Hdmi1HdcpStateFeedback
    {
        public const string Key = "Hdmi1HdcpState";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null || device.HdmiIn[1] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(Key,
                () => (int)device.HdmiIn[1].VideoAttributes.HdcpStateFeedback);

            device.HdmiIn[1].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class Hdmi1CurrentResolutionFeedback
    {
        public const string Key = "Hdmi1CurrentResolution";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if(device.HdmiIn == null || device.HdmiIn[1] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(Key, () =>
            {
                var resolution = string.Format("{0}x{1}@{2}", device.HdmiIn[1].VideoAttributes.HorizontalResolutionFeedback.UShortValue, device.HdmiIn[1].VideoAttributes.VerticalResolutionFeedback.UShortValue, device.HdmiIn[1].VideoAttributes.FramesPerSecondFeedback.UShortValue);
                return resolution;
            });

            device.HdmiIn[1].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class Hdmi1AudioChannelsFeedback
    {
        public const string Key = "Hdmi1AudioChannels";

        public static IntFeedback GetFeedback(DmNvxBaseClass device)
        {
            if(device.HdmiIn == null | device.HdmiIn[1] == null)
            {
                return new IntFeedback(() => 0);
            }

            var feedback = new IntFeedback(Key, () => device.HdmiIn[1].AudioChannelsFeedback.UShortValue);

            device.HdmiIn[1].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class Hdmi1AudioFormatFeedback
    {
        public const string Key = "Hdmi1AudioFormat";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null | device.HdmiIn[1] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(Key, () => device.HdmiIn[1].AudioFormatFeedback.ToString());

            device.HdmiIn[1].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class Hdmi1ColorSpaceFeedback
    {
        public const string Key = "Hdmi1Colorspace";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null | device.HdmiIn[1] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(Key, () => device.HdmiIn[1].VideoAttributes.ColorSpaceFeedback.ToString());

            device.HdmiIn[1].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class Hdmi1HdrTypeFeedback
    {
        public const string Key = "Hdmi1Colorspace";

        public static StringFeedback GetFeedback(DmNvxBaseClass device)
        {
            if (device.HdmiIn == null | device.HdmiIn[1] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(Key, () => device.HdmiIn[1].HdrTypeFeedback.ToString());

            device.HdmiIn[1].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }
}