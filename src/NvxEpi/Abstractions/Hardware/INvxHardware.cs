using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware
{
    public interface INvxHardware 
    {
        DmNvxBaseClass Hardware { get; }
    }

    public interface INvxDirector
    {
        DmXioDirectorBase Hardware { get; }
    }
}