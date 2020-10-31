using NvxEpi.Abstractions.Hardware;

namespace NvxEpi.Abstractions.InputSwitching
{
    public interface IAudioInput : INvxHardware
    {
        void SetAudioInput(ushort input);
        void SetAudioToHdmiInput1();
        void SetAudioToHdmiInput2();
        void SetAudioToInputAnalog();
        void SetAudioToPrimaryStreamAudio();
        void SetAudioToSecondaryStreamAudio();
        void SetAudioToInputAutomatic();
    }
}