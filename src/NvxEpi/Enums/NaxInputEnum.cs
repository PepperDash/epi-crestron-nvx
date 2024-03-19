
using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class NaxInputEnum : Enumeration<NaxInputEnum>
    {
        private NaxInputEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly NaxInputEnum AudioFollowsVideo = new NaxInputEnum(0, "AudioFollowsVideo");
        public static readonly NaxInputEnum Hdmi1 = new NaxInputEnum(1, "Hdmi1");
        public static readonly NaxInputEnum Hdmi2 = new NaxInputEnum(2, "Hdmi2");
        public static readonly NaxInputEnum AnalogAudio = new NaxInputEnum(3, "AnalogAudio");
        public static readonly NaxInputEnum Stream = new NaxInputEnum(4, "Stream");
    }
}