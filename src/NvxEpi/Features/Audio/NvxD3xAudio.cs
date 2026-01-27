using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Audio;

public class NvxD3xAudio : IBasicVolumeWithFeedback
{
    private readonly DmNvxD3x _device;
    private readonly IKeyed _parent;

    public NvxD3xAudio(DmNvxD3x device, IKeyed parent)
    {
        _device = device;
        _parent = parent;

        MuteFeedback = new BoolFeedback(
            "Muted",
            () => _device.Control.AudioMutedFeedback.BoolValue
        );

        VolumeLevelFeedback = new IntFeedback(
            "Volume",
            () =>
            {
                var volume = _device.Control.AnalogAudioOutputVolumeFeedback.ShortValue;
                var result = MapVolume(volume);
                return result;
            }
        );

        _device.OnlineStatusChange += (@base, args) => MuteFeedback.FireUpdate();
        _device.OnlineStatusChange += (@base, args) => VolumeLevelFeedback.FireUpdate();

        _device.BaseEvent += (@base, args) => MuteFeedback.FireUpdate();
        _device.BaseEvent += (@base, args) => VolumeLevelFeedback.FireUpdate();
    }

    public static int MapVolume(short level)
    {
        const float inputMin = -800;
        const float inputMax = 240;

        const float outputMin = 0;
        const float outputMax = ushort.MaxValue;

        var normalized = (level - inputMin) / (inputMax - inputMin);
        var mappedValue = (int)(normalized * (outputMax - outputMin) + outputMin);

        return mappedValue;
    }

    public void VolumeUp(bool pressRelease)
    {
        _parent.LogWarning("Volume press not implemented");
    }

    public void VolumeDown(bool pressRelease)
    {
        _parent.LogWarning("Volume press not implemented");
    }

    public void MuteToggle()
    {
        if (_device.Control.AudioMutedFeedback.BoolValue)
        {
            _device.Control.AudioUnmute();
        }
        else
        {
            _device.Control.AudioMute();
        }
    }

    public void SetVolume(ushort level)
    {
        var volume = CrestronEnvironment.ScaleWithLimits(
            level,
            ushort.MaxValue,
            ushort.MinValue,
            240,
            -800
        );
        _device.Control.AnalogAudioOutputVolume.ShortValue = (short)volume;
    }

    public void MuteOn()
    {
        _device.Control.AudioMute();
    }

    public void MuteOff()
    {
        _device.Control.AudioUnmute();
    }

    public IntFeedback VolumeLevelFeedback { get; private set; }
    public BoolFeedback MuteFeedback { get; private set; }

    public string Name => _parent is IKeyed keyedParent ? keyedParent.Key : _parent.Key;
    public string Key => _parent.Key;
}
