using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Device.Enums;
using NvxEpi.Device.JoinMaps;
using NvxEpi.Device.Services.FeedbackExtensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Services.TrilistExtensions
{
    public static class TrilistFeedbackExtensions
    {
        public static BasicTriList BuildFeedbackList(this BasicTriList trilist,
            FeedbackCollection<Feedback> feedbacks, NvxDeviceJoinMap joinMap)
        {
            foreach (var feedback in feedbacks)
            {
                uint joinNumber = 0;
                if (feedback.Key == DeviceNameFeedback.Key)
                    joinNumber = joinMap.DeviceName.JoinNumber;

                if (feedback.Key == DeviceStatusFeedback.Key)
                    joinNumber = joinMap.DeviceStatus.JoinNumber;

                if (feedback.Key == SecondaryAudioStatusFeedback.Key)
                    joinNumber = joinMap.SecondaryAudioStatus.JoinNumber;

                if (feedback.Key == StreamUrlFeedback.Key)
                    joinNumber = joinMap.StreamUrl.JoinNumber;

                if (feedback.Key == InputSyncDetectedFeedback.Hdmi1Key)
                    joinNumber = joinMap.Hdmi1SyncDetected.JoinNumber;

                if (feedback.Key == InputSyncDetectedFeedback.Hdmi2Key)
                    joinNumber = joinMap.Hdmi2SyncDetected.JoinNumber;

                if (feedback.Key == InputHdcpCapabilityFeedback.Hdmi1NameKey)
                    joinNumber = joinMap.Hdmi1Capability.JoinNumber;

                if (feedback.Key == InputHdcpCapabilityFeedback.Hdmi2NameKey)
                    joinNumber = joinMap.Hdmi2Capability.JoinNumber;

                if (feedback.Key == InputHdcpCapabilityFeedback.Hdmi1ValueKey)
                    joinNumber = joinMap.Hdmi1Capability.JoinNumber;

                if (feedback.Key == InputHdcpCapabilityFeedback.Hdmi2ValueKey)
                    joinNumber = joinMap.Hdmi2Capability.JoinNumber;

                if (feedback.Key == HdmiOutputDisabledFeedback.Key)
                    joinNumber = joinMap.HdmiOutputDisableByHdcp.JoinNumber;

                if (feedback.Key == VideoInputFeedback.NameKey)
                    joinNumber = joinMap.VideoInput.JoinNumber;

                if (feedback.Key == VideoInputFeedback.ValueKey)
                    joinNumber = joinMap.VideoInput.JoinNumber;

                if (feedback.Key == AudioInputFeedback.NameKey)
                    joinNumber = joinMap.AudioInput.JoinNumber;

                if (feedback.Key == AudioInputFeedback.ValueKey)
                    joinNumber = joinMap.AudioInput.JoinNumber;

                if (feedback.Key == MulticastAddressFeedback.Key)
                    joinNumber = joinMap.MulticastVideoAddress.JoinNumber;

                if (feedback.Key == SecondaryAudioAddressFeedback.Key)
                    joinNumber = joinMap.MulticastAudioAddress.JoinNumber;

                if (feedback.Key == HorizontalResolutionFeedback.Key) { }

                if (joinNumber > 0)
                    trilist.LinkFeedback(feedback, joinNumber);
            }

            return trilist;
        }

        private static void LinkFeedback(this BasicTriList trilist, Feedback feedback, uint join)
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
    }
}