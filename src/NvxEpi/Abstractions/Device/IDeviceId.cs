namespace NvxEpi.Abstractions.Device
{
    public interface IDeviceId
    {
        int DeviceId { get; }
        void UpdateDeviceId(uint id);
    }
}