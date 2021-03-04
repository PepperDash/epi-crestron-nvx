namespace NvxEpi.Abstractions.Device
{
    public interface ITransmitterReceiver : IDeviceMode
    {
        bool IsTransmitter { get; }
    }
}