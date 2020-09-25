using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvx35XHardware : INvxDevice
    {
        new DmNvx35x Hardware { get; }
    }
}