using Crestron.SimplSharp;
using NvxEpi.Device.Models.Device;

namespace NvxEpi.Device.Abstractions
{
    public interface IHasHdmiInputs
    {
        ReadOnlyDictionary<uint, DeviceHdmiInput> HdmiInputs { get; }
    }
}