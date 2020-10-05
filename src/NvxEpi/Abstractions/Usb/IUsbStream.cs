using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Usb
{
    public interface IUsbStream : INvx35XHardware
    {
        bool IsRemote { get; }
        StringFeedback UsbAddress { get; }
        int UsbId { get; }
    }
}