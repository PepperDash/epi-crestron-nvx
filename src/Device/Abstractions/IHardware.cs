using Crestron.SimplSharpPro.DM.Streaming;

namespace NvxEpi.Device.Abstractions
{
    public interface IHardware
    {
        DmNvxBaseClass Hardware { get; }
    }

    public interface INvx35x : IHardware
    {
        new DmNvx35x Hardware { get; }
    }
}