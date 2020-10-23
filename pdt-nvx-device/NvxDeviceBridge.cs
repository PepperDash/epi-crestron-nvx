using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Bridges;

namespace NvxEpi
{
    public static class NvxDeviceBridge
    {
        public static void LinkToApiExt(this NvxDeviceEpi device, Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist, uint joinStart, string joinMapKey)
        {
            var joinMap = new NvxDeviceJoinMap(joinStart);

            device.IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline]);

            var streamStartedFb = device.StreamStartedFb as BoolFeedback;
            if (streamStartedFb != null) streamStartedFb.LinkInputSig(trilist.BooleanInput[joinMap.StreamStarted]);

            var hdmi01SyncDetectedFb = device.HdmiInput1SyncDetectedFb as BoolFeedback;
            if (hdmi01SyncDetectedFb != null) hdmi01SyncDetectedFb.LinkInputSig(trilist.BooleanInput[joinMap.Hdmi01SyncDetected]);

            var hdmi02SyncDetectedFb = device.HdmiInput2SyncDetectedFb as BoolFeedback;
            if (hdmi02SyncDetectedFb != null) hdmi02SyncDetectedFb.LinkInputSig(trilist.BooleanInput[joinMap.Hdmi02SyncDetected]);

            var videoSourceFb = device.VideoSourceFb as IntFeedback;
            if (videoSourceFb != null) videoSourceFb.LinkInputSig(trilist.UShortInput[joinMap.VideoSource]);

            var audioSourceFb = device.AudioSourceFb as IntFeedback;
            if (audioSourceFb != null) audioSourceFb.LinkInputSig(trilist.UShortInput[joinMap.AudioSource]);

            var videoInputSourceFb = device.VideoInputSourceFb as IntFeedback;
            if (videoInputSourceFb != null) videoInputSourceFb.LinkInputSig(trilist.UShortInput[joinMap.VideoInputSource]);

            var audioInputSourceFb = device.AudioInputSourceFb as IntFeedback;
            if (audioInputSourceFb != null) audioInputSourceFb.LinkInputSig(trilist.UShortInput[joinMap.AudioInputSource]);

            var deviceModeFb = device.DeviceModeFb as IntFeedback;
            if (deviceModeFb != null) deviceModeFb.LinkInputSig(trilist.UShortInput[joinMap.DeviceMode]);

            var hdmi01HdcpCapabilityFb = device.HdmiInput1HdmiCapabilityFb as IntFeedback;
            if (hdmi01HdcpCapabilityFb != null) hdmi01HdcpCapabilityFb.LinkInputSig(trilist.UShortInput[joinMap.Hdmi01HdcpCapability]);

            var hdmi02HdcpCapabilityFb = device.HdmiInput2HdmiCapabilityFb as IntFeedback;
            if (hdmi02HdcpCapabilityFb != null) hdmi02HdcpCapabilityFb.LinkInputSig(trilist.UShortInput[joinMap.Hdmi02HdcpCapability]);

            var hdmi01HdcpSupportedLevelFb = device.HdmiInput1SupportedLevelFb as IntFeedback;
            if (hdmi01HdcpSupportedLevelFb != null) hdmi01HdcpSupportedLevelFb.LinkInputSig(trilist.UShortInput[joinMap.Hdmi01HdcpSupportedLevel]);

            var hdmi02HdcpSupportedLevelFb = device.HdmiInput2SupportedLevelFb as IntFeedback;
            if (hdmi02HdcpSupportedLevelFb != null) hdmi02HdcpSupportedLevelFb.LinkInputSig(trilist.UShortInput[joinMap.Hdmi02HdcpSupportedLevel]);

            var hdmiOutputResolutionFb = device.OutputResolutionFb as IntFeedback;
            if (hdmiOutputResolutionFb != null) hdmiOutputResolutionFb.LinkInputSig(trilist.UShortInput[joinMap.HdmiOuputResolution]);

            var videoWallModeFb = device.VideoWallModeFb as IntFeedback;
            if (videoWallModeFb != null) videoWallModeFb.LinkInputSig(trilist.UShortInput[joinMap.VideowallMode]);

            var nameFb = device.DeviceNameFb as StringFeedback;
            if (nameFb != null) nameFb.LinkInputSig(trilist.StringInput[joinMap.DeviceName]);

            var deviceStatusFb = device.DeviceStatusFb as StringFeedback;
            if (deviceStatusFb != null) deviceStatusFb.LinkInputSig(trilist.StringInput[joinMap.DeviceStatus]);

            var streamUrlFeedback = device.StreamUrlFb as StringFeedback;
            if (streamUrlFeedback != null) streamUrlFeedback.LinkInputSig(trilist.StringInput[joinMap.StreamUrl]);

            var multicastVideoAddressFb = device.MulticastVideoAddressFb as StringFeedback;
            if (multicastVideoAddressFb != null) multicastVideoAddressFb.LinkInputSig(trilist.StringInput[joinMap.MulticastVideoAddress]);

            var multicastAudioAddressFb = device.MulticastAudioAddressFb as StringFeedback;
            if (multicastAudioAddressFb != null) multicastAudioAddressFb.LinkInputSig(trilist.StringInput[joinMap.MulticastAudioAddress]);

            var currentlyRoutedVideoFb = device.CurrentlyRoutedVideoSourceFb as StringFeedback;
            if (currentlyRoutedVideoFb != null) currentlyRoutedVideoFb.LinkInputSig(trilist.StringInput[joinMap.CurrentlyRoutedVideoSource]);

            var currentlyRoutedAudioFb = device.CurrentlyRoutedAudioSourceFb as StringFeedback;
            if (currentlyRoutedAudioFb != null) currentlyRoutedAudioFb.LinkInputSig(trilist.StringInput[joinMap.CurrentlyRoutedAudioSource]);

            trilist.SetUShortSigAction(joinMap.VideoSource, source => device.VideoSource = source);
            trilist.SetUShortSigAction(joinMap.AudioSource, source => device.AudioSource = source);
            trilist.SetUShortSigAction(joinMap.VideoInputSource, input => device.VideoInputSource = input);
            trilist.SetUShortSigAction(joinMap.AudioInputSource, input => device.AudioInputSource = input);
            trilist.SetUShortSigAction(joinMap.Hdmi01HdcpCapability, hdcp => device.HdmiInput1HdmiCapability = hdcp);
            trilist.SetUShortSigAction(joinMap.Hdmi02HdcpCapability, hdcp => device.HdmiInput2HdmiCapability = hdcp);
            trilist.SetStringSigAction(joinMap.StreamUrl, s => device.StreamUrl = s);
        }
    }

    public class NvxDeviceJoinMap : JoinMapBase
    {
        public uint DeviceOnline { get; private set; }
        public uint StreamStarted { get; private set; }
        public uint Hdmi01SyncDetected { get; private set; }
        public uint Hdmi02SyncDetected { get; private set; }

        public uint VideoSource { get; private set; }
        public uint AudioSource { get; private set; }
        public uint VideoInputSource { get; private set; }
        public uint AudioInputSource { get; private set; }
        public uint DeviceMode { get; private set; }
        public uint Hdmi01HdcpCapability { get; private set; }
        public uint Hdmi01HdcpSupportedLevel { get; private set; }
        public uint Hdmi02HdcpCapability { get; private set; }
        public uint Hdmi02HdcpSupportedLevel { get; private set; }
        public uint HdmiOuputResolution { get; private set; }
        public uint VideowallMode { get; private set; }

        public uint DeviceName { get; private set; }
        public uint DeviceStatus { get; private set; }
        public uint StreamUrl { get; private set; }
        public uint MulticastVideoAddress { get; private set; }
        public uint MulticastAudioAddress { get; private set; }
        public uint CurrentlyRoutedVideoSource { get; private set; }
        public uint CurrentlyRoutedAudioSource { get; private set; }

        NvxDeviceJoinMap()
        {
            DeviceOnline = 1;
            StreamStarted = 2;
            Hdmi01SyncDetected = 3;
            Hdmi02SyncDetected = 4;

            VideoSource = 1;
            AudioSource = 2;
            VideoInputSource = 3;
            AudioInputSource = 4;
            DeviceMode = 5;
            Hdmi01HdcpCapability = 6;
            Hdmi01HdcpSupportedLevel = 7;
            Hdmi02HdcpCapability = 8;
            Hdmi02HdcpSupportedLevel = 9;
            HdmiOuputResolution = 10;
            VideowallMode = 11;

            DeviceName = 1;
            DeviceStatus = 2;
            StreamUrl = 3;
            MulticastVideoAddress = 4;
            MulticastAudioAddress = 5;
            CurrentlyRoutedVideoSource = 6;
            CurrentlyRoutedAudioSource = 7;
        }

        public NvxDeviceJoinMap(uint joinStart)
            : this()
        {
            OffsetJoinNumbers(joinStart);
        }

        public override void OffsetJoinNumbers(uint joinStart)
        {
            var joinOffset = joinStart - 1;

            GetType().GetCType().GetProperties().Where(p => p.PropertyType == typeof(uint)).ToList().ForEach(prop =>
                {
                    prop.SetValue(this, (uint)prop.GetValue(this, null) + joinOffset, null);
                });
        }
    }
}