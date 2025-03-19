using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class VideoOutputEnum : Enumeration<VideoOutputEnum>
    {
        private VideoOutputEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly VideoOutputEnum Stream = new VideoOutputEnum(1, "Stream");
        public static readonly VideoOutputEnum Hdmi = new VideoOutputEnum(2, "Hdmi");
    }
}