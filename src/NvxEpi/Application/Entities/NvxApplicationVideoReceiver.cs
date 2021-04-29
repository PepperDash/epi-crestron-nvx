using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Application.Config;
using NvxEpi.Entities.Routing;
using NvxEpi.Entities.Streams.Video;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials.Core;

namespace NvxEpi.Application.Entities
{
    public class NvxApplicationVideoReceiver : EssentialsDevice, IOnline
    {
        private readonly IEnumerable<NvxApplicationVideoTransmitter> _transmitters;
        public int DeviceId { get; private set; }
        public INvxDevice Device { get; private set; }
        public IRoutingSink Display { get; private set; }
        public StringFeedback NameFeedback { get; private set; }
        public StringFeedback VideoName { get; private set; }
        public StringFeedback CurrentVideoRouteName { get; private set; }
        public IntFeedback CurrentVideoRouteId { get; private set; }
        public BoolFeedback DisabledByHdcp { get; private set; }
        public IntFeedback HorizontalResolution { get; private set; }
        public StringFeedback EdidManufacturer { get; private set; }

        public NvxApplicationVideoReceiver(string key, NvxApplicationDeviceVideoConfig config, int deviceId, IEnumerable<NvxApplicationVideoTransmitter> transmitters) : base(key)
        {
            _transmitters = transmitters;
            DeviceId = deviceId;
            var sink = new MockDisplay(key + "--sink", key + "--videoSink");
            Display = sink;

            AddPreActivationAction(() =>
                {
                    Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as INvxDevice;
                    if (Device == null)
                        throw new NullReferenceException("device");
                });

            AddPreActivationAction(() =>
                {
                    var port = Device.OutputPorts[SwitcherForHdmiOutput.Key];
                    if (port == null)
                        throw new NullReferenceException("hdmi output routing port");

                    TieLineCollection.Default.Add(new TieLine(port, sink.HdmiIn1));
                });

            AddPreActivationAction(() =>
                {
                    Name = Device.Name;
                    NameFeedback = new StringFeedback(() => Device.Name);
                    VideoName = new StringFeedback(() => string.IsNullOrEmpty(config.VideoName) ? Device.Name : config.VideoName);
                    NameFeedback.FireUpdate();
                    VideoName.FireUpdate();
                });

            AddPreActivationAction(() =>
                {
                    var feedback = Device.Feedbacks[CurrentVideoStream.RouteNameKey] as StringFeedback;
                    if (feedback == null)
                        throw new NullReferenceException(CurrentVideoStream.RouteNameKey);

                    var currentRouteFb = new IntFeedback(() =>
                        {
                            if (feedback.StringValue.Equals(NvxGlobalRouter.NoSourceText))
                                return 0;

                            var result = _transmitters.FirstOrDefault(t => t.Name.Equals(feedback.StringValue));
                            return result == null ? 0 : result.DeviceId;
                        });

                    feedback.OutputChange += (sender, args) => currentRouteFb.FireUpdate();
                    CurrentVideoRouteId = currentRouteFb;
                    Device.Feedbacks.Add(currentRouteFb);
                });

            AddPreActivationAction(() =>
                {
                    var currentRouteNameFb = new StringFeedback(() =>
                        {
                            if (CurrentVideoRouteId.IntValue == 0)
                                return NvxGlobalRouter.NoSourceText;

                            var result = _transmitters.FirstOrDefault(t => t.DeviceId.Equals(CurrentVideoRouteId.IntValue));
                            return result == null ? NvxGlobalRouter.NoSourceText : result.VideoName.StringValue;
                        });

                    CurrentVideoRouteId.OutputChange += (sender, args) => currentRouteNameFb.FireUpdate();
                    CurrentVideoRouteName = currentRouteNameFb;
                    Device.Feedbacks.Add(currentRouteNameFb);
                });

            AddPreActivationAction(() =>
                {
                    DisabledByHdcp = new BoolFeedback(() => false);
                    HorizontalResolution = new IntFeedback(() => 0);
                    EdidManufacturer = new StringFeedback(() => string.Empty);

                    var hdmiOut = Device as IHdmiOutput;
                    if (hdmiOut == null)
                        return;

                    DisabledByHdcp = hdmiOut.DisabledByHdcp;
                    HorizontalResolution = hdmiOut.HorizontalResolution;
                    EdidManufacturer = hdmiOut.EdidManufacturer;
                });

        }

        public BoolFeedback IsOnline
        {
            get { return Device.IsOnline; }
        }
    }
}