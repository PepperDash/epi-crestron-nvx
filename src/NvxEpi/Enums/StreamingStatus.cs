
using NvxEpi.Enums;

namespace NvxEpi.Device.Enums;

public class StreamingStatusEnum : Enumeration<StreamingStatusEnum>
{
    private StreamingStatusEnum(int value, string name)
        : base(value, name)
    {

    }

    public static readonly StreamingStatusEnum Stopped = new(0, "Stopped");
    public static readonly StreamingStatusEnum Started = new(1, "Started");
    public static readonly StreamingStatusEnum Paused = new(2, "Paused");
    public static readonly StreamingStatusEnum Connecting = new(3, "Connecting");
}