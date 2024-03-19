using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class AudioOutputEnum : Enumeration<AudioOutputEnum>
    {
        private AudioOutputEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly AudioOutputEnum Stream = new AudioOutputEnum(1, "Stream");
        public static readonly AudioOutputEnum Hdmi = new AudioOutputEnum(2, "Hdmi");
        public static readonly AudioOutputEnum Analog = new AudioOutputEnum(3, "Analog");
    }
}