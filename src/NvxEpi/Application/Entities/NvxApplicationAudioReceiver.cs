using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Application.Config;
using NvxEpi.Entities.Routing;
using NvxEpi.Entities.Streams.Audio;
using NvxEpi.Entities.Streams.Video;
using NvxEpi.Services.InputSwitching;
using PepperDash.Core;
using PepperDash.Essentials;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Entities
{
    public class NvxApplicationAudioReceiver : EssentialsDevice
    {
        private readonly IEnumerable<NvxApplicationAudioTransmitter> _transmitters;
        public int DeviceId { get; private set; }
        public INvxDevice Device { get; private set; }
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

            AddPreActivationAction(() =>
                {
                    Debug.Console(1, this, "Looking for Device...");
                    Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as INvxDevice;
                    if (Device == null)
                        throw new NullReferenceException("device");
                });

            AddPreActivationAction(() =>
                {
                    Debug.Console(1, this, "Activating ports...");
                    var port = Device.OutputPorts[SwitcherForAnalogAudioOutput.Key];
                    if (port == null)
                        throw new NullReferenceException("audio output routing port");

                    TieLineCollection.Default.Add(new TieLine(port, sink.AudioIn));
                });

            AddPreActivationAction(() =>
                {
                    Debug.Console(1, this, "Setting up device name...");
                    Name = Device.Name;
                    AudioName =
                        new StringFeedback(() => string.IsNullOrEmpty(config.AudioName) ? Device.Name : config.AudioName);
                    AudioName.FireUpdate();
                });

            AddPreActivationAction(() =>
                {
                    Debug.Console(1, this, "Setting up secondary audio route id...");
                    var feedback = Device.Feedbacks[CurrentSecondaryAudioStream.RouteNameKey] as StringFeedback;
                    if (feedback == null)
                        throw new NullReferenceException(CurrentVideoStream.RouteNameKey);

                    var currentRouteFb = new IntFeedback(Key + "--appRouteAudioCurrentId",
                        () =>
                            {
                                if (feedback.StringValue.Equals(NvxGlobalRouter.NoSourceText))
                                    return 0;

                                var result = _transmitters.FirstOrDefault(t => t.Name.Equals(feedback.StringValue));
                                return result == null ? 0 : result.DeviceId;
                            });
                    feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                    CurrentAudioRouteId = currentRouteFb;
                    Device.Feedbacks.Add(currentRouteFb);
                });

            AddPreActivationAction(() =>
                {
                    Debug.Console(1, this, "Setting up secondary audio route name...");
                    var currentRouteNameFb = new StringFeedback(Key + "--appRouteAudioName",
                        () =>
                            {
                                if (CurrentAudioRouteId.IntValue == 0)
                                    return NvxGlobalRouter.NoSourceText;

                                var result =
                                    _transmitters.FirstOrDefault(t => t.DeviceId.Equals(CurrentAudioRouteId.IntValue));
                                return result == null ? NvxGlobalRouter.NoSourceText : result.AudioName.StringValue;
                            });

                    CurrentAudioRouteId.OutputChange += (sender, args) => currentRouteNameFb.FireUpdate();
                    CurrentAudioRouteName = currentRouteNameFb;
                    Device.Feedbacks.Add(currentRouteNameFb);
                });
        }
    }
}