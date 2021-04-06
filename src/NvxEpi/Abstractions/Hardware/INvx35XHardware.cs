using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvx35XHardware : INvxHardware
    {
        new DmNvx35x Hardware { get; }
    }

    public interface INvx36XHardware : INvxHardware
    {
        new DmNvx36x Hardware { get; }
    }
}