using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Services.Feedback
{
    public class HdmiSyncDetectedFeedback
    {
        public const string Key = "Hdmi{0}SyncDetected";

        public static BoolFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
                return new BoolFeedback(() => false);

            var feedback = new BoolFeedback(string.Format(Key, _inputNumber),
                () => device.HdmiIn[_inputNumber].SyncDetectedFeedback.BoolValue);

            device.HdmiIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class HdmiHdcpSupportFeedback
    {
        public const string Key = "Hdmi{0}HdcpSupport";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {           
            if(device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
            {
                return new StringFeedback(string.Format(Key, _inputNumber), () => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.HdmiIn[_inputNumber].HdcpSupportedLevelFeedback.ToString());

            device.HdmiIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiHdcpCapabilityValueFeedback
    {        
        public const string Key = "Hdmi{0}HdcpCapabilityValue";

        public static IntFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {           
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(string.Format(Key, _inputNumber),
                () => (int)device.HdmiIn[_inputNumber].HdcpCapabilityFeedback);

            device.HdmiIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }
    public class HdmiHdcpCapabilityFeedback
    {
        public const string Key = "Hdmi{0}HdcpCapability";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {           
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
                return new StringFeedback(() => string.Empty);

            var feedback = new StringFeedback(string.Format(Key, _inputNumber),
                () => device.HdmiIn[_inputNumber].HdcpCapabilityFeedback.ToString());

            device.HdmiIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class HdmiHdcpStateFeedback
    {
        public const string Key = "Hdmi{0}HdcpState";

        public static IntFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {
            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
                return new IntFeedback(() => 0);

            var feedback = new IntFeedback(string.Format(Key, _inputNumber),
                () => (int)device.HdmiIn[_inputNumber].VideoAttributes.HdcpStateFeedback);

            
            device.HdmiIn[_inputNumber].StreamChange += (stream, args) => feedback.FireUpdate();
            return feedback;
        }
    }

    public class HdmiCurrentResolutionFeedback
    {
        public const string Key = "Hdmi{0}CurrentResolution";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {
            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, _inputNumber), () =>
            {
                var resolution = string.Format("{0}x{1}@{2}", device.HdmiIn[_inputNumber].VideoAttributes.HorizontalResolutionFeedback.UShortValue, device.HdmiIn[_inputNumber].VideoAttributes.VerticalResolutionFeedback.UShortValue, device.HdmiIn[_inputNumber].VideoAttributes.FramesPerSecondFeedback.UShortValue);
                return resolution;
            });

            device.HdmiIn[_inputNumber].VideoAttributes.AttributeChange += (a, args) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiAudioChannelsFeedback
    {
        public const string Key = "Hdmi{0}AudioChannels";

        public static IntFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {
            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
            {
                return new IntFeedback(() => 0);
            }

            var feedback = new IntFeedback(string.Format(Key, _inputNumber), () => device.HdmiIn[_inputNumber].AudioChannelsFeedback.UShortValue);

            device.HdmiIn[_inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiAudioFormatFeedback
    {
        public const string Key = "Hdmi{0}AudioFormat";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {
            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.HdmiIn[_inputNumber].AudioFormatFeedback.ToString());

            device.HdmiIn[_inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiColorSpaceFeedback
    {
        public const string Key = "Hdmi{0}Colorspace";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {
            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.HdmiIn[_inputNumber].VideoAttributes.ColorSpaceFeedback.ToString());

            device.HdmiIn[_inputNumber].VideoAttributes.AttributeChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }

    public class HdmiHdrTypeFeedback
    {
        public const string Key = "Hdmi{0}HdrType";

        public static StringFeedback GetFeedback(DmNvxBaseClass device, uint _inputNumber)
        {
            
            if (device.HdmiIn == null || device.HdmiIn[_inputNumber] == null)
            {
                return new StringFeedback(() => string.Empty);
            }

            var feedback = new StringFeedback(string.Format(Key, _inputNumber), () => device.HdmiIn[_inputNumber].HdrTypeFeedback.ToString());

            device.HdmiIn[_inputNumber].StreamChange += (s, a) => feedback.FireUpdate();

            return feedback;
        }
    }
}
