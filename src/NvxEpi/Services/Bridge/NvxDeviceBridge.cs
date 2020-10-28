using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.InputSwitching;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Abstractions.Stream;
using NvxEpi.Entities.Routing;
using NvxEpi.Entities.Streams;
using NvxEpi.Extensions;
using NvxEpi.JoinMaps;
using NvxEpi.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace NvxEpi.Services.Bridge
{
    public class NvxDeviceBridge : IBridgeAdvanced
    {
        private readonly INvxHardware _device;

        public NvxDeviceBridge(INvxHardware device)
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

                if (feedback.Key == CurrentVideoStream.RouteNameKey 
                    || feedback.Key == CurrentVideoStream.RouteValueKey)
                    joinNumber = joinMap.VideoRoute.JoinNumber;

                if (feedback.Key == CurrentSecondaryAudioStream.RouteNameKey
                    || feedback.Key == CurrentSecondaryAudioStream.RouteValueKey)
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

            var stream = _device as IStream;
            if (stream != null)
                trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, stream.SetStreamUrl);
        }

        private void LinkRouting(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            if (_device.IsTransmitter) return;

            var stream = _device as IStream;
            if (stream != null)
            {
                trilist.SetUShortSigAction(joinMap.VideoRoute.JoinNumber, source => PrimaryStreamRouter.Route(source, stream));
                trilist.SetStringSigAction(joinMap.VideoRoute.JoinNumber,
                    name => PrimaryStreamRouter.Route(name, stream));
            }
            var secondaryAudio = _device as ISecondaryAudioStream;
            if (secondaryAudio == null) return;
            trilist.SetUShortSigAction(joinMap.AudioRoute.JoinNumber, source => SecondaryAudioRouter.Route(source, secondaryAudio));
            trilist.SetStringSigAction(joinMap.AudioRoute.JoinNumber,
                name => SecondaryAudioRouter.Route(name, secondaryAudio));
        }

        private void LinkVideowallMode(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            if (_device.IsTransmitter) return;

            var videowallDevice = _device as IVideowallMode;
            var videowallMode = new BoolFeedback(() => videowallDevice == null);
            videowallMode.LinkInputSig(trilist.BooleanInput[joinMap.VideowallMode.JoinNumber]);
            videowallMode.FireUpdate();

            if (!videowallMode.BoolValue) return;
            trilist.SetUShortSigAction(joinMap.VideowallMode.JoinNumber, videowallDevice.SetVideowallMode);
        }

        private void LinkHdmiInputs(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            var hdmi1Fb = new BoolFeedback(() => _device.Hardware.HdmiIn != null && _device.Hardware.HdmiIn.Count >= 1);
            hdmi1Fb.LinkInputSig(trilist.BooleanInput[joinMap.HdmiIn1Present.JoinNumber]);

            var hdmi2Fb = new BoolFeedback(() => _device.Hardware.HdmiIn != null && _device.Hardware.HdmiIn.Count >= 2);
            hdmi2Fb.LinkInputSig(trilist.BooleanInput[joinMap.HdmiIn2Present.JoinNumber]);

            var hdmiInputs = _device as IHdmiInput;
            if (hdmiInputs != null && hdmiInputs.HdcpCapability.Count >= 1)
                trilist.SetUShortSigAction(joinMap.Hdmi1Capability.JoinNumber, hdmiInputs.SetHdmi1HdcpCapability);

            if (hdmiInputs != null && hdmiInputs.HdcpCapability.Count >= 2)
                trilist.SetUShortSigAction(joinMap.Hdmi2Capability.JoinNumber, hdmiInputs.SetHdmi2HdcpCapability);
        }
    }
}