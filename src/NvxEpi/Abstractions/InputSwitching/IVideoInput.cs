using NvxEpi.Abstractions.Hardware;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface IVideoInput : INvxHardware
    {
        void SetVideoInput(ushort input);
        void SetVideoToHdmiInput1();
        void SetVideoToHdmiInput2();
        void SetVideoToNone();
        void SetVideoToStream();
    }
}