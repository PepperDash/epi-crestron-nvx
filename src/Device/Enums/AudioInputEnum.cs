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
        public static readonly AudioInputEnum Stream = new AudioInputEnum(4, "Stream");
        public static readonly AudioInputEnum NaxAudio = new AudioInputEnum(5, "Nax");
    }
}