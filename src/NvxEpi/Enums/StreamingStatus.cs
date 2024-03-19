
using NvxEpi.Enums;

namespace NvxEpi.Device.Enums
{
    public class StreamingStatusEnum : Enumeration<StreamingStatusEnum>
    {
        private StreamingStatusEnum(int value, string name)
            : base(value, name)
        {

        }

        public static readonly StreamingStatusEnum Stopped = new StreamingStatusEnum(0, "Stopped");
        public static readonly StreamingStatusEnum Started = new StreamingStatusEnum(1, "Started");
        public static readonly StreamingStatusEnum Paused = new StreamingStatusEnum(2, "Paused");
        public static readonly StreamingStatusEnum Connecting = new StreamingStatusEnum(3, "Connecting");
    }
}