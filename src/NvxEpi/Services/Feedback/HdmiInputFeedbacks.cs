using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HdmiSyncDetectedFeedback
    {
        public const string Key = "Hdmi{0}SyncDetected";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting SyncDetected Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
                return new BoolFeedback(() => false);

            var feedback = new BoolFeedback(string.Format(Key, inputNumber),
                () => device.HdmiIn[inputNumber].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class HdmiHdcpCapabilityValueFeedback
    {
        
        public const string Key = "Hdmi{0}HdcpCapabilityValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting HdcpCapabilityValue Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(string.Format(Key, inputNumber),
                () => (int)device.HdmiIn[inputNumber].HdcpCapabilityFeedback);

            device.HdmiIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
    public class HdmiHdcpCapabilityFeedback
    {
        public const string Key = "Hdmi{0}HdcpCapability";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting HdcpCapability Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
                return new StringFeedback(() => string.Empty);

            var feedback = new StringFeedback(string.Format(Key, inputNumber),
                () => device.HdmiIn[inputNumber].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class HdmiHdcpStateFeedback
    {
        public const string Key = "Hdmi{0}HdcpState";

        public static IntFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting HdcpState Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(string.Format(Key, inputNumber),
                () => (int)device.HdmiIn[inputNumber].VideoAttributes.HdcpStateFeedback);

            
            device.HdmiIn[inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class HdmiCurrentResolutionFeedback
    {
        public const string Key = "Hdmi{0}CurrentResolution";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting CurrentResolution Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, inputNumber), () =>
            {
                var resolution = $"{device.HdmiIn[inputNumber].VideoAttributes.HorizontalResolutionFeedback.UShortValue}x{device.HdmiIn[inputNumber].VideoAttributes.VerticalResolutionFeedback.UShortValue}@{device.HdmiIn[inputNumber].VideoAttributes.FramesPerSecondFeedback.UShortValue}";
                return resolution;
            });

            device.HdmiIn[inputNumber].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiAudioChannelsFeedback
    {
        public const string Key = "Hdmi{0}AudioChannels";

        public static IntFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting AudioResolution Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
            {
                return new IntFeedback(() => 0);
            }

            var feedback = new IntFeedback(string.Format(Key, inputNumber), () => device.HdmiIn[inputNumber].AudioChannelsFeedback.UShortValue);

            device.HdmiIn[inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiAudioFormatFeedback
    {
        public const string Key = "Hdmi{0}AudioFormat";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting AudioFormat Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, inputNumber), () => device.HdmiIn[inputNumber].AudioFormatFeedback.ToString());

            device.HdmiIn[inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiColorSpaceFeedback
    {
        public const string Key = "Hdmi{0}Colorspace";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting Colorspace Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, inputNumber), () => device.HdmiIn[inputNumber].VideoAttributes.ColorSpaceFeedback.ToString());

            device.HdmiIn[inputNumber].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiHdrTypeFeedback
    {
        public const string Key = "Hdmi{0}Colorspace";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint inputNumber)
        {
            Debug.Console(1, "Getting HdrType Feedback for {input}", inputNumber);
            if (device.HdmiIn == null || device.HdmiIn[inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, inputNumber), () => device.HdmiIn[inputNumber].HdrTypeFeedback.ToString());

            device.HdmiIn[inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }
}
