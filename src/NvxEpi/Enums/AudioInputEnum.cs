using NvxEpi.Enums;

namespace NvxEpi.Device.Enums;

public class AudioInputEnum : Enumeration<AudioInputEnum>
{
    private AudioInputEnum(int value, string name)
        : base(value, name)
    {

    }

    public static readonly AudioInputEnum AudioFollowsVideo = new(0, "None");
    public static readonly AudioInputEnum Hdmi1 = new(1, "Hdmi1");
    public static readonly AudioInputEnum Hdmi2 = new(2, "Hdmi2");
    public static readonly AudioInputEnum AnalogAudio = new(3, "AnalogAudio");
    public static readonly AudioInputEnum PrimaryStream = new(4, "PrimaryStream");
    public static readonly AudioInputEnum SecondaryStream = new(5, "SecondaryStream");
    public static readonly AudioInputEnum Dante = new(6, "Dante");
    public static readonly AudioInputEnum NaxAudio = new(7, "Nax");
    public static readonly AudioInputEnum Usbc1 = new(11, "Usbc1");
    public static readonly AudioInputEnum UsbC2 = new(12, "Usbc2");

    /*
    NVX-384(c) Audio input: <AudioSource>
    0 = Audio Follows Video
    1 = HDMI 1
    2 = HDMI 2
    3 = Analog Audio
    4 = Primary Stream
    5 = DM NAX (AES67)
    11 = USB-C 1
    12 = USB-C 2
    */
}