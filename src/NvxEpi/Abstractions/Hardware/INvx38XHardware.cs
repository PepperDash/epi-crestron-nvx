using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware;

public interface INvx38XHardware
{
    DmNvx38x Hardware { get; }
}