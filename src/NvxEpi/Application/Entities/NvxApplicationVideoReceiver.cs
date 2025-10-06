using System;
using System.Collections.Generic;
using System.Linq;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Application.Config;
using NvxEpi.Features.Routing;
using NvxEpi.Features.Streams.Video;
using NvxEpi.Services.InputSwitching;
using PepperDash.Essentials.Core;
using MockDisplay = PepperDash.Essentials.Devices.Common.Displays.MockDisplay;

namespace NvxEpi.Application.Entities;

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
    public IntFeedback AspectRatioMode { get; private set; }

    public NvxApplicationVideoReceiver(string key, NvxApplicationDeviceVideoConfig config, int deviceId, IEnumerable<NvxApplicationVideoTransmitter> transmitters) : base(key)
    {
        _transmitters = transmitters;
        DeviceId = deviceId;
        var sink = new MockDisplay(key + "--sink", key + "--videoSink");
        Display = sink;

        AddPostActivationAction(() =>
            {
                Device = DeviceManager.GetDeviceForKey(config.DeviceKey) as INvxDevice;
                if (Device == null)
                    throw new NullReferenceException("device");
            });

        AddPostActivationAction(() =>
            {
                var port = Device.OutputPorts[SwitcherForHdmiOutput.Key] ?? throw new NullReferenceException("hdmi output routing port");
                TieLineCollection.Default.Add(new TieLine(port, sink.InputPorts[RoutingPortNames.HdmiIn1]));
            });

        AddPostActivationAction(() =>
            {
                Name = Device.Name;
                NameFeedback = new StringFeedback("name", () => Device.Name);
                VideoName = new StringFeedback("videoName", () => string.IsNullOrEmpty(config.VideoName) ? Device.Name : config.VideoName);
                NameFeedback.FireUpdate();
                VideoName.FireUpdate();
            });

        AddPostActivationAction(() =>
            {
                var feedback = Device.Feedbacks[CurrentVideoStream.RouteNameKey] as StringFeedback ?? throw new NullReferenceException(CurrentVideoStream.RouteNameKey);

                var currentRouteFb = new IntFeedback("currentRoute", () =>
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

        AddPostActivationAction(() =>
            {
                var currentRouteNameFb = new StringFeedback("currentRouteName", () =>
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

        AddPostActivationAction(() =>
            {
                DisabledByHdcp = new BoolFeedback("disabledByHdcp", () => false);
                HorizontalResolution = new IntFeedback("horizontalResolution", () => 0);
                AspectRatioMode = new IntFeedback("aspectRatioMode", () => 0);
                EdidManufacturer = new StringFeedback("edidManufacturer", () => string.Empty);

                if (Device is not IHdmiOutput hdmiOut)
                    return;

                DisabledByHdcp = hdmiOut.DisabledByHdcp;
                HorizontalResolution = hdmiOut.HorizontalResolution;
                EdidManufacturer = hdmiOut.EdidManufacturer;

                if (Device is not IVideowallMode aspect)
                    return;

                AspectRatioMode = aspect.VideoAspectRatioMode;
            });
    }

    public BoolFeedback IsOnline
    {
        get { return Device.IsOnline; }
    }
}