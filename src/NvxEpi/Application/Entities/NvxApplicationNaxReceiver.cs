using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Application.Config;
using NvxEpi.Devices;
using NvxEpi.Features.Routing;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Entities;

/// <summary>
/// A DM-NAX-AUD-IO as an application audio output. The route value is the video transmitter number, and the
/// unit receives that encoder's secondary audio stream.
/// </summary>
public class NvxApplicationNaxReceiver : EssentialsDevice
{
    private readonly IEnumerable<NvxApplicationVideoTransmitter> _transmitters;
    public int DeviceId { get; private set; }
    public DmNaxAudIo Device { get; private set; }
    public StringFeedback AudioName { get; private set; }
    public IntFeedback CurrentAudioRouteId { get; private set; }
    public StringFeedback CurrentAudioRouteName { get; private set; }

    public NvxApplicationNaxReceiver(string key, NvxApplicationDeviceAudioConfig config, int deviceId,
                                     IEnumerable<NvxApplicationVideoTransmitter> transmitters)
        : base(key)
    {
        _transmitters = transmitters;
        DeviceId = deviceId;

        AddPostActivationAction(() =>
            {
                Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as DmNaxAudIo
                         ?? throw new NullReferenceException(string.Format("dmNaxAudIo at key : {0}", config.DeviceKey));

                Name = Device.Name;
                AudioName = new StringFeedback("audioName", () => string.IsNullOrEmpty(config.AudioName) ? Device.Name : config.AudioName);
                AudioName.FireUpdate();

                CurrentAudioRouteId = new IntFeedback(Key + "--appRouteNaxCurrentId", () => CurrentTransmitter()?.DeviceId ?? 0);
                CurrentAudioRouteName = new StringFeedback(Key + "--appRouteNaxName",
                    () => CurrentTransmitter()?.VideoName.StringValue ?? NvxGlobalRouter.NoSourceText);

                Device.CurrentRouteValue.OutputChange += (sender, args) =>
                    {
                        CurrentAudioRouteId.FireUpdate();
                        CurrentAudioRouteName.FireUpdate();
                    };
            });
    }

    private NvxApplicationVideoTransmitter CurrentTransmitter()
    {
        var txDeviceId = Device.CurrentRouteValue.IntValue;
        if (txDeviceId == 0)
            return null;

        return _transmitters.FirstOrDefault(
            t => t.Device is ISecondaryAudioStream tx && tx.IsTransmitter && tx.DeviceId == txDeviceId);
    }

    /// <summary>Route the given video transmitter's audio; null clears.</summary>
    public void Route(NvxApplicationVideoTransmitter transmitter)
    {
        Device.RouteFrom(transmitter?.Device as ISecondaryAudioStream);
    }
}
