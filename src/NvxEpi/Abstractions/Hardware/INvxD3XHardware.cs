using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxD3XHardware : INvxHardware
    {
        new DmNvxD3x Hardware { get; }
    }
}