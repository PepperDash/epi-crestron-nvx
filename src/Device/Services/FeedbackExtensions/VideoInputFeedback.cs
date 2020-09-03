using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Services.FeedbackExtensions
{
    public static class VideoInputFeedback
    {
        public static readonly string ValueKey = "VideoInputFeedbackValue";
        public static readonly string NameKey = "VideoInputFeedback";

        public static StringFeedback GetVideoInputFeedback(this DmNvxBaseClass device)
        {
            var feedback = new StringFeedback(NameKey,
                () => device.Control.ActiveVideoSourceFeedback.ToString());

            device.BaseEvent += (@base, args) => feedback.FireUpdate();

            return feedback;
        }

        public static IntFeedback GetVideoInputValueFeedback(this DmNvxBaseClass device)
        {
            var feedback = new IntFeedback(ValueKey,
                () => (int) device.Control.ActiveVideoSourceFeedback);

            device.BaseEvent += (@base, args) => feedback.FireUpdate();
            return feedback;
        }

        /*public static void SetTxVideoInput(this DmNvxBaseClass device, ushort input)
        {
            VideoInputEnum result;
            if (!VideoInputEnum.TryFromValue(input, out result))
                return;

            if (result == VideoInputEnum.Stream)
                return;

            device.Control.VideoSource = (eSfpVideoSourceTypes) result.Value;
        }

        public static void SetRxVideoInput(this DmNvxBaseClass device, ushort input)
        {
            VideoInputEnum result;
            if (!VideoInputEnum.TryFromValue(input, out result))
                return;

            device.Control.VideoSource = (eSfpVideoSourceTypes)result.Value;
        }*/
    }
}