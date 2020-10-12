using System;
using PepperDash.Essentials.Core;
using DmChassisControllerJoinMap = PepperDash.Essentials.Core.Bridges.DmChassisControllerJoinMap;

namespace NvxEpi.Application.JoinMap
{
    public class NvxApplicationJoinMap : JoinMapBaseAdvanced
    {
        public NvxApplicationJoinMap(uint joinStart) : base(joinStart)
        {
            var dmJoinMap = new DmChassisControllerJoinMap(joinStart);

            EnableAudioBreakaway = dmJoinMap.EnableAudioBreakaway;
            EnableUsbBreakaway = dmJoinMap.EnableUsbBreakaway;
            VideoSyncStatus = dmJoinMap.VideoSyncStatus;
            InputEndpointOnline = dmJoinMap.InputEndpointOnline;
            OutputEndpointOnline = dmJoinMap.OutputEndpointOnline;
            TxAdvancedIsPresent = dmJoinMap.TxAdvancedIsPresent;
            OutputDisabledByHdcp = dmJoinMap.OutputDisabledByHdcp;
            OutputVideo = dmJoinMap.OutputVideo;
            OutputAudio = dmJoinMap.OutputAudio;
            OutputUsb = dmJoinMap.OutputUsb;
            HdcpSupportState = dmJoinMap.HdcpSupportState;
            HdcpSupportCapability = dmJoinMap.HdcpSupportCapability;
            InputNames = dmJoinMap.InputNames;
            OutputNames = dmJoinMap.OutputNames;
            InputVideoNames = dmJoinMap.InputVideoNames;
            InputAudioNames = dmJoinMap.InputAudioNames;
            OutputVideoNames = dmJoinMap.OutputVideoNames;
            OutputAudioNames = dmJoinMap.OutputAudioNames;
            OutputCurrentVideoInputNames = dmJoinMap.OutputCurrentVideoInputNames;
            OutputCurrentAudioInputNames = dmJoinMap.OutputCurrentAudioInputNames;
            InputCurrentResolution = dmJoinMap.InputCurrentResolution;
        }

        [JoinName("EnableAudioBreakaway")] 
        public JoinDataComplete EnableAudioBreakaway;

        [JoinName("EnableUsbBreakaway")]
        public JoinDataComplete EnableUsbBreakaway;

        [JoinName("VideoSyncStatus")]
        public JoinDataComplete VideoSyncStatus;

        [JoinName("InputEndpointOnline")]
        public JoinDataComplete InputEndpointOnline;

        [JoinName("OutputEndpointOnline")]
        public JoinDataComplete OutputEndpointOnline;

        [JoinName("TxAdvancedIsPresent")]
        public JoinDataComplete TxAdvancedIsPresent;

        [JoinName("OutputDisabledByHdcp")]
        public JoinDataComplete OutputDisabledByHdcp;

        [JoinName("OutputVideo")]
        public JoinDataComplete OutputVideo;

        [JoinName("OutputAudio")]
        public JoinDataComplete OutputAudio;

        [JoinName("OutputUsb")]
        public JoinDataComplete OutputUsb;

        [JoinName("HdcpSupportState")]
        public JoinDataComplete HdcpSupportState;

        [JoinName("HdcpSupportCapability")]
        public JoinDataComplete HdcpSupportCapability;

        [JoinName("InputNames")]
        public JoinDataComplete InputNames;

        [JoinName("OutputNames")]
        public JoinDataComplete OutputNames;

        [JoinName("InputVideoNames")]
        public JoinDataComplete InputVideoNames;

        [JoinName("InputAudioNames")]
        public JoinDataComplete InputAudioNames;

        [JoinName("OutputVideoNames")]
        public JoinDataComplete OutputVideoNames;

        [JoinName("OutputAudioNames")]
        public JoinDataComplete OutputAudioNames;

        [JoinName("OutputCurrentVideoInputNames")]
        public JoinDataComplete OutputCurrentVideoInputNames;

        [JoinName("OutputCurrentAudioInputNames")]
        public JoinDataComplete OutputCurrentAudioInputNames;

        [JoinName("InputCurrentResolution")]
        public JoinDataComplete InputCurrentResolution;

        [JoinName("OutputHorizontalResolution")] 
        public JoinDataComplete OutputHorizontalResolution = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 3301,
                JoinSpan = 32
            },
            new JoinMetadata()
            {
                Description = "Analog value of horizontal resolution on HDMI output",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog
            });

        [JoinName("ReceiverSerialPorts")]
        public JoinDataComplete ReceiverSerialPorts = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 3901,
                JoinSpan = 32
            },
            new JoinMetadata()
            {
                Description = "COM TX/RX for receivers if registered",
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Serial
            });
    }
}