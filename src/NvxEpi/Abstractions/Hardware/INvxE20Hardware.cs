using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Abstractions.Hardware;

public interface INvxE20Hardware : INvxHardware
{
    new DmNvxE20 Hardware { get; }
}