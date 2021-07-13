namespace NvxEpi.Enums
{
    public class DeviceInputEnum : Enumeration<DeviceInputEnum>
    {
        private DeviceInputEnum(int value, string name) : base(value, name)
        {
            
        }

        public static readonly DeviceInputEnum Stream = new DeviceInputEnum(0, "Stream");
        public static readonly DeviceInputEnum Hdmi1 = new DeviceInputEnum(1, "Hdmi1");
        public static readonly DeviceInputEnum Hdmi2 = new DeviceInputEnum(2, "Hdmi2");
        public static readonly DeviceInputEnum AnalogAudio = new DeviceInputEnum(3, "AnalogAudio");
        public static readonly DeviceInputEnum PrimaryAudio = new DeviceInputEnum(4, "PrimaryAudio");
        public static readonly DeviceInputEnum SecondaryAudio = new DeviceInputEnum(5, "SecondaryAudio");
        public static readonly DeviceInputEnum DanteAudio = new DeviceInputEnum(6, "DanteAudio");
        public static readonly DeviceInputEnum DmNaxAudio = new DeviceInputEnum(7, "DmNaxAudio");
        public static readonly DeviceInputEnum Automatic = new DeviceInputEnum(98, "Automatic");
        public static readonly DeviceInputEnum NoSwitch = new DeviceInputEnum(99, "NoSwitch");
    }
}