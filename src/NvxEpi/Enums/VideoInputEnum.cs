using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class VideoInputEnum : Enumeration<VideoInputEnum>
    {
        private VideoInputEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly VideoInputEnum None = new VideoInputEnum(0, "None");
        public static readonly VideoInputEnum Hdmi1 = new VideoInputEnum(1, "Hdmi1");
        public static readonly VideoInputEnum Hdmi2 = new VideoInputEnum(2, "Hdmi2");
        public static readonly VideoInputEnum Stream = new VideoInputEnum(3, "Stream");
    }
}