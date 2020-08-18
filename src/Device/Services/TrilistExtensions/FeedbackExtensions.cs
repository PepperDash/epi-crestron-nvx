using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Device.JoinMaps;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Services.TrilistExtensions
{
    public static class FeedbackExtensions
    {
        public static BasicTriList BuildFeedbackList(this BasicTriList trilist,
            Dictionary<NvxDevice.DeviceFeedbacks, Feedback> feedbacks, NvxDeviceJoinMap joinMap)
        {
            foreach (var feedback in feedbacks)
            {
                uint joinNumber = 0;
                switch (feedback.Key)
                {
                    case NvxDevice.DeviceFeedbacks.DeviceName:
                        joinNumber = joinMap.DeviceName.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.DeviceStatus:
                        joinNumber = joinMap.DeviceStatus.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.DeviceMode:
                        break;

                    case NvxDevice.DeviceFeedbacks.SecondaryAudioStatus:
                        joinNumber = joinMap.SecondaryAudioStatus.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.NaxTxStatus:
                        throw new NotSupportedException("NaxTx");

                    case NvxDevice.DeviceFeedbacks.NaxRxStatus:
                        throw new NotSupportedException("NaxRx");

                    case NvxDevice.DeviceFeedbacks.StreamUrl:
                        joinNumber = joinMap.StreamUrl.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.Hdmi1SyncDetected:
                        joinNumber = joinMap.Hdmi1SyncDetected.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.Hdmi2SyncDetected:
                        joinNumber = joinMap.Hdmi2SyncDetected.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.Hdmi1HdcpCapabilityName:
                        joinNumber = joinMap.Hdmi1Capability.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.Hdmi2HdcpCapabilityName:
                        joinNumber = joinMap.Hdmi2Capability.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.HdmiOutputDisabledByHdcp:
                        joinNumber = joinMap.HdmiOutputDisableByHdcp.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.VideoInputName:
                        joinNumber = joinMap.VideoInput.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.VideoInputValue:
                        joinNumber = joinMap.VideoInput.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.AudioInputName:
                        joinNumber = joinMap.AudioInput.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.AudioInputValue:
                        joinNumber = joinMap.AudioInput.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.NaxInput:
                        throw new NotSupportedException("Nax");

                    case NvxDevice.DeviceFeedbacks.MulticastAddress:
                        joinNumber = joinMap.MulticastVideoAddress.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.SecondaryAudioAddress:
                        joinNumber = joinMap.MulticastAudioAddress.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.NaxTxAddress:
                        throw new NotSupportedException("Nax");

                    case NvxDevice.DeviceFeedbacks.NaxRxAddress:
                        throw new NotSupportedException("Nax");

                    case NvxDevice.DeviceFeedbacks.VideowallMode:
                        throw new NotImplementedException("videowall mode");

                    case NvxDevice.DeviceFeedbacks.CurrentVideoRouteName:
                        joinNumber = joinMap.VideoRoute.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.CurrentAudioRouteName:
                        joinNumber = joinMap.AudioRoute.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.CurrentUsbRouteName:
                        throw new NotImplementedException("usb route name");

                    case NvxDevice.DeviceFeedbacks.CurrentUsbRouteValue:
                        throw new NotImplementedException("usb route value");

                    case NvxDevice.DeviceFeedbacks.UsbMode:
                        throw new NotImplementedException("usb mode");

                    case NvxDevice.DeviceFeedbacks.Hdmi1HdcpCapabilityValue:
                        joinNumber = joinMap.Hdmi1Capability.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.Hdmi2HdcpCapabilityValue:
                        joinNumber = joinMap.Hdmi2Capability.JoinNumber;
                        break;

                    case NvxDevice.DeviceFeedbacks.HdmiOutputHorizontalResolution:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(feedback.Key.ToString());
                }

                if (joinNumber > 0)
                    trilist.LinkFeedback(feedback.Value, joinNumber);
            }

            return trilist;
        }

        private static void LinkFeedback(this BasicTriList trilist, Feedback feedback, uint join)
        {
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