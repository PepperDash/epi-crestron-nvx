using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxE76XHardware : INvxHardware
    {
        new DmNvxE760x Hardware { get; }
    }
}