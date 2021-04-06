using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxD8XHardware : INvxHardware
    {
        new DmNvxD80Ioav Hardware { get; }
    }
}