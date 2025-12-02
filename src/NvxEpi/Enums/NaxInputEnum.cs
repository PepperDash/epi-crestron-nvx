
using NvxEpi.Enums;

namespace NvxEpi.Device.Enums;

public class NaxInputEnum : Enumeration<NaxInputEnum>
{
    private NaxInputEnum(int value, string name)
        : base(value, name)
    {

    }

    public static readonly NaxInputEnum AudioFollowsVideo = new(0, "AudioFollowsVideo");
    public static readonly NaxInputEnum Hdmi1 = new(1, "Hdmi1");
    public static readonly NaxInputEnum Hdmi2 = new(2, "Hdmi2");
    public static readonly NaxInputEnum AnalogAudio = new(3, "AnalogAudio");
    public static readonly NaxInputEnum Stream = new(4, "Stream");
    public static readonly NaxInputEnum Usbc1 = new(5, "Usbc1");
    public static readonly NaxInputEnum Usbc2 = new(6, "Usbc2");
}