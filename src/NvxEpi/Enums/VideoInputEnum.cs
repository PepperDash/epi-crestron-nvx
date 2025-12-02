using NvxEpi.Enums;

namespace NvxEpi.Device.Enums;

public class VideoInputEnum : Enumeration<VideoInputEnum>
{
    private VideoInputEnum(int value, string name)
        : base(value, name)
    {

    }

    public static readonly VideoInputEnum None = new(0, "None");
    public static readonly VideoInputEnum Hdmi1 = new(1, "Hdmi1");
    public static readonly VideoInputEnum Hdmi2 = new(2, "Hdmi2");
    public static readonly VideoInputEnum Stream = new(3, "Stream");
    public static readonly VideoInputEnum Usbc1 = new(4, "Usbc1");
    public static readonly VideoInputEnum Usbc2 = new(5, "Usbc2");
}