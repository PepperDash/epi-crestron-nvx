using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Usb
{
    public interface IUsbStream : INvxDevice
    {
        bool IsRemote { get; }
        StringFeedback UsbId { get; }
    }
}