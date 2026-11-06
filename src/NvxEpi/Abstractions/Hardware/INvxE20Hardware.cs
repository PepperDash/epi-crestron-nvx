using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware;

public interface INvxE20Hardware : INvxHardware
{
    new DmNvxE10E20Base Hardware { get; }
}