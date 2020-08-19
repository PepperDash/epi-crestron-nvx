using System;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;
using NvxEpi.Device.Services.DeviceExtensions;
using PepperDash.Core;
using PepperDash.Essentials.Core;
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

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.HdmiOutputDisabledByHdcp,
                Device.GetHdmiOutputDisabledFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.HdmiOutputHorizontalResolution,
                Device.GetHorizontalResolutionFeedback());

            if (IsTransmitter) return;
            Feedbacks.Add(NvxDevice.DeviceFeedbacks.CurrentVideoRouteName,
                Device.GetCurrentVideoRouteNameFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.CurrentAudioRouteName,
                Device.GetSecondaryAudioRouteNameFeedback());
        }

        private void AddActions()
        {
            if (IsTransmitter)
            {
                IntActions.Add(NvxDevice.IntActions.VideoInputSelect, Device.SetTxVideoInput);
                IntActions.Add(NvxDevice.IntActions.AudioInputSelect, Device.SetAudioTxInput);
            }
            else
            {
                IntActions.Add(NvxDevice.IntActions.VideoInputSelect, Device.SetRxVideoInput);
                IntActions.Add(NvxDevice.IntActions.AudioInputSelect, Device.SetAudioRxInput);

                StringActions.Add(NvxDevice.StringActions.StreamUrl,
                    s => Device.Control.ServerUrl.StringValue = s);

                StringActions.Add(NvxDevice.StringActions.SecondaryAudioAddress,
                    s => Device.SecondaryAudio.MulticastAddress.StringValue = s);
            }

            IntActions.Add(NvxDevice.IntActions.Hdmi1HdcpCapability,
                capability => Device.HdmiIn[1].HdcpCapability = (eHdcpCapabilityType) capability);

            IntActions.Add(NvxDevice.IntActions.Hdmi2HdcpCapability,
                capability => Device.HdmiIn[2].HdcpCapability = (eHdcpCapabilityType) capability);  

            BoolActions.Add(NvxDevice.BoolActions.EnableVideoStream, enable =>
            {
                if (enable)
                    Device.Control.Start();
                else
                    Device.Control.Stop();
            });

            BoolActions.Add(NvxDevice.BoolActions.EnableAudioStream, enable =>
            {
                if (enable)
                    Device.SecondaryAudio.Start();
                else
                    Device.SecondaryAudio.Stop();
            });
        }

        public override void SetDeviceDefaults()
        {
            if (IsTransmitter)
                SetTxDefaults();
            else
                SetRxDefaults();

            try
            {
                if (!String.IsNullOrEmpty(_props.DefaultVideoSource))
                {
                    var videoInput =
                        (eSfpVideoSourceTypes)Enum.Parse(typeof(eSfpVideoSourceTypes), _props.DefaultVideoSource, true);

                    Debug.Console(1, this, "Setting default video input:{0}", videoInput.ToString());
                    Device.Control.VideoSource = videoInput;
                }

                if (String.IsNullOrEmpty(_props.DefaultAudioSource)) 
                    return;

                var audioInput =
                    (DmNvxControl.eAudioSource)
                        Enum.Parse(typeof(DmNvxControl.eAudioSource), _props.DefaultAudioSource, true);

                Debug.Console(1, this, "Setting default audio input:{0}", audioInput.ToString());
                Device.Control.AudioSource = audioInput;
            }
            catch (ArgumentException ex)
            {
                Debug.Console(1, this, "Cannot set default input, argument not resolved:{0}", ex.Message);
            }
        }

        protected override void BuildRoutingPorts(NvxDevice device)
        {
            device.InputPorts.AddRange(new[]
            {
                new RoutingInputPort(
                    VideoInputEnum.Hdmi1.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    new Action(() =>
                    {
                        Device.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
                        Device.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
                    }),
                    device),

                new RoutingInputPort(
                    VideoInputEnum.Hdmi2.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    new Action(() =>
                    {
                        Device.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
                        Device.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
                    }),
                    device),
            });

            device.OutputPorts.Add(
                new RoutingOutputPort(
                    VideoOutputEnum.Hdmi.Name,
                    eRoutingSignalType.AudioVideo,
                    eRoutingPortConnectionType.Hdmi,
                    null,
                    device));

            if (IsTransmitter)
            {
                device.InputPorts.Add(
                    new RoutingInputPort(
                        AudioInputEnum.AnalogAudio.Name,
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.LineAudio,
                        new Action(() => Device.Control.AudioSource = DmNvxControl.eAudioSource.AnalogAudio),
                        device));

                device.OutputPorts.Add(
                    new RoutingOutputPort(VideoOutputEnum.Stream.Name,
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        null,
                        device));
            }
            else
            {
                device.InputPorts.Add(
                    new RoutingInputPort(
                        VideoInputEnum.Stream.Name,
                        eRoutingSignalType.AudioVideo,
                        eRoutingPortConnectionType.Streaming,
                        new Action(() =>
                        {
                            Device.Control.VideoSource = eSfpVideoSourceTypes.Stream;
                            Device.Control.AudioSource = DmNvxControl.eAudioSource.Automatic;
                        }),
                        device));

                device.OutputPorts.Add(
                    new RoutingOutputPort(
                        AudioOutputEnum.Analog.Name,
                        eRoutingSignalType.Audio,
                        eRoutingPortConnectionType.Streaming,
                        null,
                        device));
            }
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