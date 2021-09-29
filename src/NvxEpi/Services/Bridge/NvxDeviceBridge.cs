﻿using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Extensions;
using NvxEpi.Features.Routing;
using NvxEpi.Features.Streams.Audio;
using NvxEpi.Features.Streams.Video;
using NvxEpi.JoinMaps;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace NvxEpi.Services.Bridge
{
    public class NvxDeviceBridge : IBridgeAdvanced
    {
        private readonly INvxDevice _device;

        public NvxDeviceBridge(INvxDevice device)
        {
            _device = device;
        }

        private static void BuildFeedbackList(BasicTriList trilist,
            IEnumerable<PepperDash.Essentials.Core.Feedback> feedbacks, NvxDeviceJoinMap joinMap)
        {
            foreach (var feedback in feedbacks)
            {
                uint joinNumber = 0;

                if (feedback.Key == DeviceNameFeedback.Key)
                    joinNumber = joinMap.DeviceName.JoinNumber;
 
                if (feedback.Key == IsStreamingVideoFeedback.Key)
                    joinNumber = joinMap.StreamStarted.JoinNumber;

                if (feedback.Key == Hdmi1SyncDetectedFeedback.Key)
                    joinNumber = joinMap.Hdmi1SyncDetected.JoinNumber;

                if (feedback.Key == Hdmi2SyncDetectedFeedback.Key)
                    joinNumber = joinMap.Hdmi2SyncDetected.JoinNumber;

                if (feedback.Key == Hdmi1HdcpCapabilityValueFeedback.Key)
                    joinNumber = joinMap.Hdmi1Capability.JoinNumber;

                if (feedback.Key == Hdmi1HdcpCapabilityValueFeedback.Key)
                    joinNumber = joinMap.Hdmi2Capability.JoinNumber;

                if (feedback.Key == Hdmi1HdcpCapabilityFeedback.Key)
                    joinNumber = joinMap.Hdmi1Capability.JoinNumber;

                if (feedback.Key == Hdmi2HdcpCapabilityFeedback.Key)
                    joinNumber = joinMap.Hdmi2Capability.JoinNumber;

                if (feedback.Key == HdmiOutputDisabledFeedback.Key)
                    joinNumber = joinMap.HdmiOutputDisableByHdcp.JoinNumber;

                if (feedback.Key == VideowallModeFeedback.Key)
                    joinNumber = joinMap.VideowallMode.JoinNumber;

                if (feedback.Key == CurrentVideoStream.RouteNameKey)
                    joinNumber = joinMap.VideoRouteString.JoinNumber;

                if (feedback.Key == CurrentVideoStream.RouteValueKey)
                    joinNumber = joinMap.VideoRoute.JoinNumber;

                if (feedback.Key == CurrentSecondaryAudioStream.RouteNameKey)
                    joinNumber = joinMap.AudioRouteString.JoinNumber;

                if (feedback.Key == CurrentSecondaryAudioStream.RouteValueKey)
                    joinNumber = joinMap.AudioRoute.JoinNumber;

                if (feedback.Key == VideoInputFeedback.Key)
                    joinNumber = joinMap.VideoInput.JoinNumber;

                if (feedback.Key == VideoInputValueFeedback.Key)
                    joinNumber = joinMap.VideoInput.JoinNumber;

                if (feedback.Key == AudioInputFeedback.Key)
                    joinNumber = joinMap.AudioInput.JoinNumber;

                if (feedback.Key == AudioInputValueFeedback.Key)
                    joinNumber = joinMap.AudioInput.JoinNumber;

                if (feedback.Key == StreamUrlFeedback.Key)
                    joinNumber = joinMap.StreamUrl.JoinNumber;

                if (feedback.Key == MulticastAddressFeedback.Key)
                    joinNumber = joinMap.MulticastVideoAddress.JoinNumber;

                if (feedback.Key == SecondaryAudioAddressFeedback.Key)
                    joinNumber = joinMap.MulticastAudioAddress.JoinNumber;

                if (feedback.Key == AudioTxAddressFeedback.Key)
                    joinNumber = joinMap.NaxTxAddress.JoinNumber;

                if (feedback.Key == AudioRxAddressFeedback.Key)
                    joinNumber = joinMap.NaxRxAddress.JoinNumber;

                if (feedback.Key == DanteInputFeedback.Key)
                    joinNumber = joinMap.DanteInput.JoinNumber;

                if (feedback.Key == DanteInputValueFeedback.Key)
                    joinNumber = joinMap.DanteInput.JoinNumber;

                if (feedback.Key == NaxInputFeedback.Key)
                    joinNumber = joinMap.NaxInput.JoinNumber;

                if (feedback.Key == NaxInputValueFeedback.Key)
                    joinNumber = joinMap.NaxInput.JoinNumber;

                if (feedback.Key == VideoAspectRatioModeFeedback.Key)
                    joinNumber = joinMap.VideoAspectRatioMode.JoinNumber;

                if (feedback.Key == HorizontalResolutionFeedback.Key) { }

                if (joinNumber > 0)
                    LinkFeedback(trilist, feedback, joinNumber);
            }
        }

        private static void LinkFeedback(BasicTriList trilist, IKeyed feedback, uint join)
        {
            Debug.Console(1, feedback, "Linking to Trilist : {0} | Join : {0}", trilist.ID, join);

            var stringFeedback = feedback as StringFeedback;
            if (stringFeedback != null)
                stringFeedback.LinkInputSig(trilist.StringInput[join]);

            var intFeedback = feedback as IntFeedback;
            if (intFeedback != null)
                intFeedback.LinkInputSig(trilist.UShortInput[join]);

            var boolFeedback = feedback as BoolFeedback;
            if (boolFeedback != null)
                boolFeedback.LinkInputSig(trilist.BooleanInput[join]);
        }

        public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new NvxDeviceJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(_device.Key, joinMap);

            var customJoins = JoinMapHelper.TryGetJoinMapAdvancedForDevice(joinMapKey);
            if (customJoins != null)
                joinMap.SetCustomJoinData(customJoins);

            _device.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);

            BuildFeedbackList(trilist, _device.Feedbacks, joinMap);
            LinkHdmiInputs(trilist, joinMap);
            LinkVideowallMode(trilist, joinMap);
            LinkRouting(trilist, joinMap);

            var videoInput = _device as ICurrentVideoInput;
            if (videoInput != null)
                trilist.SetUShortSigAction(joinMap.VideoInput.JoinNumber, videoInput.SetVideoInput);

            var audioInput = _device as ICurrentAudioInput;
            if (audioInput != null)
                trilist.SetUShortSigAction(joinMap.AudioInput.JoinNumber, audioInput.SetAudioInput);
                
            var naxInput = _device as ICurrentNaxInput;
            if (naxInput != null)
                trilist.SetUShortSigAction(joinMap.NaxInput.JoinNumber, naxInput.SetNaxInput);

            var danteInput = _device as ICurrentDanteInput;
            if (audioInput != null)
                trilist.SetUShortSigAction(joinMap.DanteInput.JoinNumber, danteInput.SetDanteInput);

            var stream = _device as IStream;
            if (stream != null)
                trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, stream.SetStreamUrl);
        }

        private void LinkRouting(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            if (!_device.IsTransmitter)
            {
                var stream = _device as IStream;
                if (stream != null)
                {
                    trilist.SetUShortSigAction(joinMap.VideoRoute.JoinNumber, source => PrimaryStreamRouter.Route(source, stream));
                    trilist.SetStringSigAction(joinMap.VideoRoute.JoinNumber,
                        name => PrimaryStreamRouter.Route(name, stream));
                }
            }
            var secondaryAudio = _device as ISecondaryAudioStreamWithHardware;
            if (secondaryAudio == null) return;
            trilist.SetUShortSigAction(joinMap.AudioRoute.JoinNumber, source => SecondaryAudioRouter.Route(source, secondaryAudio));
            trilist.SetStringSigAction(joinMap.AudioRoute.JoinNumber,
                name => SecondaryAudioRouter.Route(name, secondaryAudio));
        }

        private void LinkVideowallMode(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            if (_device.IsTransmitter) return;

            var videowallDevice = _device as IVideowallMode;
            var videowallMode = new BoolFeedback(() => videowallDevice != null);
            videowallMode.LinkInputSig(trilist.BooleanInput[joinMap.VideowallMode.JoinNumber]);
            videowallMode.FireUpdate();

            if (!videowallMode.BoolValue) return;
            trilist.SetUShortSigAction(joinMap.VideowallMode.JoinNumber, videowallDevice.SetVideowallMode);
        }

        private void LinkHdmiInputs(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            var hdmiInput = _device as IHdmiInput;
            if (hdmiInput == null)
                return;

            var hdmi1Fb = new BoolFeedback(() => hdmiInput.SyncDetected.ContainsKey(1));
            hdmi1Fb.LinkInputSig(trilist.BooleanInput[joinMap.HdmiIn1Present.JoinNumber]);
            hdmi1Fb.FireUpdate();

            var hdmi2Fb = new BoolFeedback(() => hdmiInput.SyncDetected.ContainsKey(2));
            hdmi2Fb.LinkInputSig(trilist.BooleanInput[joinMap.HdmiIn2Present.JoinNumber]);
            hdmi2Fb.FireUpdate();

            if (hdmiInput.HdcpCapability.ContainsKey(1))
                trilist.SetUShortSigAction(joinMap.Hdmi1Capability.JoinNumber, hdmiInput.SetHdmi1HdcpCapability);

            if (hdmiInput.HdcpCapability.ContainsKey(2))
                trilist.SetUShortSigAction(joinMap.Hdmi2Capability.JoinNumber, hdmiInput.SetHdmi2HdcpCapability);
        }
    }
}