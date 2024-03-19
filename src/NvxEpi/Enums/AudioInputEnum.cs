using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class AudioInputEnum : Enumeration<AudioInputEnum>
    {
        private AudioInputEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly AudioInputEnum AudioFollowsVideo = new AudioInputEnum(0, "None");
        public static readonly AudioInputEnum Hdmi1 = new AudioInputEnum(1, "Hdmi1");
        public static readonly AudioInputEnum Hdmi2 = new AudioInputEnum(2, "Hdmi2");
        public static readonly AudioInputEnum AnalogAudio = new AudioInputEnum(3, "AnalogAudio");
        public static readonly AudioInputEnum PrimaryStream = new AudioInputEnum(4, "PrimaryStream");
        public static readonly AudioInputEnum SecondaryStream = new AudioInputEnum(5, "SecondaryStream");
        public static readonly AudioInputEnum Dante = new AudioInputEnum(6, "Dante");
        public static readonly AudioInputEnum NaxAudio = new AudioInputEnum(7, "Nax");
    }
}