using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Device.Abstractions
{
    public interface IHardware
    {
        DmNvxBaseClass Hardware { get; }
    }
}