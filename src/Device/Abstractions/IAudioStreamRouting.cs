using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Abstractions
{
    public interface IAudioStreamRouting : IAudioStream
    {
        IntFeedback CurrentAudioRouteValue { get; }
        StringFeedback CurrentAudioRouteName { get; }

        void SetAudioAddress(string address);
        void StartAudioStream();
        void StopAudioStream();
    }
}