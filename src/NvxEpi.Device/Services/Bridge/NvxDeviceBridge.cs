
using System.Collections.Generic;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Abstractions.Device;
using NvxEpi.Abstractions.Extensions;
using NvxEpi.Abstractions.HdmiInput;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.SecondaryAudio;
using NvxEpi.Device.Entities.Aggregates;
using NvxEpi.Device.Entities.Routing;
using NvxEpi.Device.Entities.Streams;
using NvxEpi.Device.JoinMaps;
using NvxEpi.Device.Services.DeviceFeedback;
using NvxEpi.Device.Services.Feedback;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;

namespace NvxEpi.Device.Services.Bridge
{
    public class NvxDeviceBridge : IBridgeAdvanced
    {
        private readonly NvxBaseDevice _device;

        public NvxDeviceBridge(NvxBaseDevice device)
        {
            _device = device;
        }

        private static void BuildFeedbackList(BasicTriList trilist,
            IEnumerable<PepperDash.Essentials.Core.Feedback> feedbacks, NvxDeviceJoinMap joinMap)
        {
            foreach (var feedback in feedbacks)
            {
                uint joinNumber = 0;

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

            var nameFb = new StringFeedback(() => _device.Name);
            nameFb.FireUpdate();

            _device.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);

            BuildFeedbackList(trilist, _device.Feedbacks, joinMap);
            LinkHdmiInputs(trilist, joinMap);
            LinkVideowallMode(trilist, joinMap);
            LinkRouting(trilist, joinMap);

            trilist.SetUShortSigAction(joinMap.VideoInput.JoinNumber, _device.SetAudioInput);
            trilist.SetUShortSigAction(joinMap.AudioInput.JoinNumber, _device.SetAudioInput);
            trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, _device.SetStreamUrl);
        }

        private void LinkRouting(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
            if (_device.IsTransmitter) return;
            trilist.SetUShortSigAction(joinMap.VideoRoute.JoinNumber, _device.RouteStream);
            trilist.SetStringSigAction(joinMap.VideoRoute.JoinNumber,
                name => PrimaryStreamRouter.Route(name, _device));

            var secondaryAudio = _device as ISecondaryAudioStream;
            if (secondaryAudio == null) return;
            trilist.SetUShortSigAction(joinMap.AudioRoute.JoinNumber, secondaryAudio.RouteSecondaryAudio);
            trilist.SetStringSigAction(joinMap.AudioRoute.JoinNumber,
                name => SecondaryAudioRouter.Route(name, secondaryAudio));
        }

        private void LinkVideowallMode(BasicTriList trilist, NvxDeviceJoinMap joinMap)
        {
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