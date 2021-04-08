using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvx36XHardware : INvxHardware
    {
        new DmNvx36x Hardware { get; }
    }
}