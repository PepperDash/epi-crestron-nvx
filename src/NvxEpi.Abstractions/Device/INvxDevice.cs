using Crestron.SimplSharpPro;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Abstractions.Device
{
    public interface INvxDevice : IKeyName, IRoutingInputsOutputs, INvxHardware, IHasFeedback
    {
        int DeviceId { get; }    
        StringFeedback MulticastAddress { get; }
    }

    public interface ICurrentVideoInput : INvxDevice
    {
        StringFeedback CurrentVideoInput { get; }
        IntFeedback CurrentVideoInputValue { get; }
    }

    public interface ICurrentAudioInput : INvxDevice
    {
        StringFeedback CurrentAudioInput { get; }
        IntFeedback CurrentAudioInputValue { get; }
    }

    public interface ITransmitterReceiver : IDeviceMode
    {
        bool IsTransmiter { get; }
    }

    public interface IDeviceMode
    {
        IntFeedback DeviceMode { get; }
    }
}