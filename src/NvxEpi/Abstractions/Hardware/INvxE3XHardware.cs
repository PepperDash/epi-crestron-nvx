using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxE3XHardware : INvxHardware
    {
        new DmNvxE3x Hardware { get; }
    }
}