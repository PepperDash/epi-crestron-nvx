using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Application.Config;
using NvxEpi.Extensions;
using NvxEpi.Features.Routing;
using NvxEpi.Features.Streams.Audio;
using NvxEpi.Services.Feedback;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Entities;

public class NvxApplicationAudioReceiver : EssentialsDevice
{
    private readonly IEnumerable<NvxApplicationAudioTransmitter> _transmitters;
    public int DeviceId { get; private set; }
    public INvxDeviceWithHardware Device { get; private set; }
    public IRoutingSink Amp { get; private set; }
    public StringFeedback AudioName { get; private set; }
    public StringFeedback CurrentAudioRouteName { get; private set; }
    public IntFeedback CurrentAudioRouteId { get; private set; }

    public NvxApplicationAudioReceiver(string key, NvxApplicationDeviceAudioConfig config, int deviceId,
                                       IEnumerable<NvxApplicationAudioTransmitter> transmitters)
        : base(key)
    {
        _transmitters = transmitters;
        DeviceId = deviceId;
        var sink = new Amplifier(key + "--amp", key + "--amp");
        Amp = sink;

        AddPostActivationAction(() =>
            {
                Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as INvxDeviceWithHardware;
                if (Device == null)
                    throw new NullReferenceException(string.Format("device at key : {0}", config.DeviceKey));
            });

        AddPostActivationAction(() =>
            {
                var port = Device.OutputPorts[SwitcherForAnalogAudioOutput.Key] ?? throw new NullReferenceException("audio output routing port");
                TieLineCollection.Default.Add(new TieLine(port, sink.AudioIn));
            });

        AddPostActivationAction(() =>
            {
                Name = Device.Name;
                AudioName =
                    new StringFeedback("audioName", () => string.IsNullOrEmpty(config.AudioName) ? Device.Name : config.AudioName);
                AudioName.FireUpdate();
            });

        AddPostActivationAction(() =>
            {
                if (Device.Feedbacks[CurrentSecondaryAudioStream.RouteNameKey] is not StringFeedback feedback)
                    throw new NullReferenceException(CurrentSecondaryAudioStream.RouteNameKey);

                if (Device.Feedbacks[AudioInputFeedback.Key] is not StringFeedback audioSourceFeedback)
                    throw new NullReferenceException(AudioInputFeedback.Key);

                var currentRouteFb = new IntFeedback(Key + "--appRouteAudioCurrentId",
                    () =>
                        {
                            if (AudioInputExtensions.AudioInputIsLocal(Device))    //If local audio is active, feedback is this unit itself
                            {
                                var self = _transmitters.FirstOrDefault(t => t.Name.Equals(this.Name));
                                return self == null ? 0 : self.DeviceId;
                            }
                            if (feedback.StringValue.Equals(NvxGlobalRouter.NoSourceText))
                                return 0;
                            var result = _transmitters.FirstOrDefault(t => t.Name.Equals(feedback.StringValue));
                            return result == null ? 0 : result.DeviceId;
                        });

                feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                audioSourceFeedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                CurrentAudioRouteId = currentRouteFb;
                Device.Feedbacks.Add(currentRouteFb);
            });

        AddPostActivationAction(() =>
            {
                var audioSourceFeedback = Device.Feedbacks[AudioInputFeedback.Key] as StringFeedback ?? throw new NullReferenceException(AudioInputFeedback.Key);
                var currentRouteNameFb = new StringFeedback(Key + "--appRouteAudioName",
                    () =>
                        {
                            if (AudioInputExtensions.AudioInputIsLocal(Device))    //If local audio is active, feedback is this unit itself
                            {
                                var self = _transmitters.FirstOrDefault(t => t.DeviceId.Equals(this.CurrentAudioRouteId.IntValue));
                                return self == null ? NvxGlobalRouter.NoSourceText : self.AudioName.StringValue;
                            }

                            if (CurrentAudioRouteId.IntValue == 0)
                                return NvxGlobalRouter.NoSourceText;

                            var result = _transmitters.FirstOrDefault(t => t.DeviceId.Equals(CurrentAudioRouteId.IntValue));
                            return result == null ? NvxGlobalRouter.NoSourceText : result.AudioName.StringValue;
                        });

                CurrentAudioRouteId.OutputChange += (sender, args) => currentRouteNameFb.FireUpdate();
                audioSourceFeedback.OutputChange += (sender, args) => currentRouteNameFb.FireUpdate();
                CurrentAudioRouteName = currentRouteNameFb;
                Device.Feedbacks.Add(currentRouteNameFb);
            });
    }
}