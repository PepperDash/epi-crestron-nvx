using Crestron.SimplSharp;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Usb
{
    public interface IUsbStream : INvxDevice
    {
        bool IsRemote { get; }
        ReadOnlyDictionary<uint, StringFeedback> UsbRemoteIds { get; }
    }

    public interface IUsbStreamWithHardware : IUsbStream, INvxHardware
    {
        StringFeedback UsbLocalId { get; }
        void MakeUsbRoute(IUsbStreamWithHardware hardware);
        void ClearCurrentUsbRoute();
    }
}