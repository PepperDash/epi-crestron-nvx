using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware;

public interface INvx38XHardware : INvxHardware
{
    new DmNvx38x Hardware { get; }
}