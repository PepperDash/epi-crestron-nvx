using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxE3XHardware : INvxHardware
    {
        new DmNvxE3x Hardware { get; }
    }

    public interface INvxD3XHardware : INvxHardware
    {
        new DmNvxD3x Hardware { get; }
    }

    public interface INvxD8XHardware : INvxHardware
    {
        new DmNvxD80Ioav Hardware { get; }
    }

    public interface INvxE76xHardware : INvxHardware
    {
        new DmNvxE760x Hardware { get; }
    }
}