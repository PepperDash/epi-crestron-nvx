using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models;
using NvxEpi.Device.Services.Config;
using NvxEpi.Device.Services.DeviceExtensions;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Builders
{
    public class Nvx35xV2Builder : NvxDeviceV2Builder
    {
        public Nvx35xV2Builder(DeviceConfig config) : base(config)
        {
            AddFeedbacks();
            AddActions();

            Device.OnlineStatusChange += (device, args) =>
            {
                if (!args.DeviceOnLine)
                    return;

                if (IsTransmitter)
                    SetTxDefaults();
                else
                    SetRxDefaults();
            };
        }

        private void AddFeedbacks()
        {
            Feedbacks.Add(NvxDevice.DeviceFeedbacks.VideoInputName,
                Device.GetVideoInputFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.AudioInputName,
                Device.GetAudioInputFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.VideoInputValue,
                Device.GetVideoInputValueFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.AudioInputValue,
                Device.GetAudioInputValueFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.SecondaryAudioAddress,
                Device.GetSecondaryAudioAddressFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.SecondaryAudioStatus,
                Device.GetSecondaryAudioStatusFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.Hdmi1HdcpCapabilityName,
                Device.GetHdmiIn1HdcpCapabilityFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.Hdmi1HdcpCapabilityValue,
                Device.GetHdmiIn1HdcpCapabilityValueFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.Hdmi2HdcpCapabilityName,
                Device.GetHdmiIn2HdcpCapabilityFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.Hdmi2HdcpCapabilityValue,
                Device.GetHdmiIn2HdcpCapabilityValueFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.Hdmi1SyncDetected,
                Device.GetHdmiIn1SyncDetectedFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.Hdmi2SyncDetected,
                Device.GetHdmiIn2SyncDetectedFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.CurrentVideoRouteName,
                Device.GetCurrentVideoRouteNameFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.CurrentAudioRouteName,
                Device.GetSecondaryAudioRouteNameFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.HdmiOutputDisabledByHdcp,
                Device.GetHdmiOutputDisabledFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.HdmiOutputHorizontalResolution,
                Device.GetHorizontalResolutionFeedback());
        }

        private void AddActions()
        {
            IntActions.Add(NvxDevice.IntActions.VideoInputSelect,
                input => Device.Control.VideoSource = (eSfpVideoSourceTypes) input);

            IntActions.Add(NvxDevice.IntActions.AudioInputSelect,
                input => Device.Control.AudioSource = (DmNvxControl.eAudioSource) input);

            IntActions.Add(NvxDevice.IntActions.Hdmi1HdcpCapability,
                capability => Device.HdmiIn[1].HdcpCapability = (eHdcpCapabilityType) capability);

            IntActions.Add(NvxDevice.IntActions.Hdmi2HdcpCapability,
                capability => Device.HdmiIn[2].HdcpCapability = (eHdcpCapabilityType) capability);

            if (IsTransmitter) 
                return;

            StringActions.Add(NvxDevice.StringActions.RouteVideo, Device.RouteVideo);
            StringActions.Add(NvxDevice.StringActions.RouteAudio, Device.RouteSecondaryAudio);

            StringActions.Add(NvxDevice.StringActions.StreamUrl, 
                s => Device.Control.ServerUrl.StringValue = s);

            StringActions.Add(NvxDevice.StringActions.SecondaryAudioAddress, 
                s => Device.SecondaryAudio.MulticastAddress.StringValue = s);
        }

        public override void SetDeviceDefaults()
        {
            if (!String.IsNullOrEmpty(_props.DefaultVideoSource))
            {
                var videoInput = (eSfpVideoSourceTypes) Enum.Parse(typeof(eSfpVideoSourceTypes), _props.DefaultVideoSource, true);
                Device.Control.VideoSource = videoInput;
            }

            if (!String.IsNullOrEmpty(_props.DefaultAudioSource))
            {
                var videoInput = (DmNvxControl.eAudioSource) Enum.Parse(typeof(DmNvxControl.eAudioSource), _props.DefaultAudioSource, true);
                Device.Control.AudioSource = videoInput;
            }

            if (IsTransmitter)
                SetTxDefaults();
            else
                SetRxDefaults();
        }

        private void SetTxDefaults()
        {
            Device.Control.DeviceMode = eDeviceMode.Transmitter;
            Device.Control.EnableAutomaticInitiation();
            Device.SecondaryAudio.EnableAutomaticInitiation();

            if (!String.IsNullOrEmpty(_props.MulticastVideoAddress))
                Device.Control.MulticastAddress.StringValue = _props.MulticastVideoAddress;

            if (!String.IsNullOrEmpty(_props.MulticastAudioAddress))
            {
                Device.SecondaryAudio.SecondaryAudioMode =
                    DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
                Device.SecondaryAudio.MulticastAddress.StringValue = _props.MulticastAudioAddress;
            }
            else
            {
                Device.SecondaryAudio.SecondaryAudioMode =
                    DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Automatic;
            }
        }

        private void SetRxDefaults()
        {
            Device.Control.DeviceMode = eDeviceMode.Receiver;
            Device.Control.DisableAutomaticInitiation();
            Device.SecondaryAudio.DisableAutomaticInitiation();
            Device.SecondaryAudio.SecondaryAudioMode = DmNvxBaseClass.DmNvx35xSecondaryAudio.eSecondaryAudioMode.Manual;
        }
    }
}