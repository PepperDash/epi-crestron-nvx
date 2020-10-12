using PepperDash.Essentials.Core;
using DmChassisControllerJoinMap = PepperDash.Essentials.Core.Bridges.DmChassisControllerJoinMap;

namespace NvxEpi.Application.JoinMap
{
    public class NvxApplicationJoinMap : JoinMapBaseAdvanced
    {
        public NvxApplicationJoinMap(uint joinStart) : base(joinStart)
        {
            var dmJoinMap = new DmChassisControllerJoinMap(joinStart);

            EnableAudioBreakaway = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.EnableAudioBreakaway.JoinNumber,
                    JoinSpan = dmJoinMap.EnableAudioBreakaway.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Enables Audio Breakaway",
                    JoinCapabilities = eJoinCapabilities.FromSIMPL,
                    JoinType = eJoinType.Digital
                });

            VideoSyncStatus = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.VideoSyncStatus.JoinNumber,
                    JoinSpan = dmJoinMap.VideoSyncStatus.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "High if sync is detected on HDMI1 of Transmitter",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

            InputEndpointOnline = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.InputEndpointOnline.JoinNumber,
                    JoinSpan = dmJoinMap.InputEndpointOnline.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Input Endpoint Online",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

            OutputEndpointOnline = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputEndpointOnline.JoinNumber,
                    JoinSpan = dmJoinMap.OutputEndpointOnline.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Output Endpoint Online",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

            TxAdvancedIsPresent = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.TxAdvancedIsPresent.JoinNumber,
                    JoinSpan = dmJoinMap.TxAdvancedIsPresent.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "TX Advanced preset",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

            OutputDisabledByHdcp = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputDisabledByHdcp.JoinNumber,
                    JoinSpan = dmJoinMap.OutputDisabledByHdcp.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "High when Hdmi out on RX is disabled by HDCP",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

            OutputVideo = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputVideo.JoinNumber,
                    JoinSpan = dmJoinMap.OutputVideo.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Routes video from a tx to rx",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Analog
                });

            OutputAudio = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputAudio.JoinNumber,
                    JoinSpan = dmJoinMap.OutputAudio.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Routes audio from a tx to rx",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Analog
                });

            OutputUsb = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputUsb.JoinNumber,
                    JoinSpan = dmJoinMap.OutputUsb.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Routes USB from a remote to local device NOT YET IMPLEMENTED",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Analog
                });

            HdcpSupportState = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.HdcpSupportState.JoinNumber,
                    JoinSpan = dmJoinMap.HdcpSupportState.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Sets and gets the HDCP Supported State",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Analog,
                    ValidValues = new [] {"0", "1", "2", "3", "99"}
                });

            HdcpSupportCapability = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.HdcpSupportCapability.JoinNumber,
                    JoinSpan = dmJoinMap.HdcpSupportCapability.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the Supported HDCP Capability",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Analog
                });

            InputNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.InputNames.JoinNumber,
                    JoinSpan = dmJoinMap.InputNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the TX Name",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            OutputNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputNames.JoinNumber,
                    JoinSpan = dmJoinMap.OutputNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the Rx Name",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            InputVideoNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.InputVideoNames.JoinNumber,
                    JoinSpan = dmJoinMap.InputVideoNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the TX Input Video Name",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            InputAudioNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.InputAudioNames.JoinNumber,
                    JoinSpan = dmJoinMap.InputAudioNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the TX Input Audio Name",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            OutputVideoNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputVideoNames.JoinNumber,
                    JoinSpan = dmJoinMap.OutputVideoNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the RX Output Video Name",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            OutputAudioNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputAudioNames.JoinNumber,
                    JoinSpan = dmJoinMap.OutputAudioNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the RX Output Audio Name",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            OutputCurrentVideoInputNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputCurrentVideoInputNames.JoinNumber,
                    JoinSpan = dmJoinMap.OutputCurrentVideoInputNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the name of the current video source",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            OutputCurrentAudioInputNames = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.OutputCurrentAudioInputNames.JoinNumber,
                    JoinSpan = dmJoinMap.OutputCurrentAudioInputNames.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the name of the current audio source",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });

            InputCurrentResolution = new JoinDataComplete(
                new JoinData()
                {
                    JoinNumber = dmJoinMap.InputCurrentResolution.JoinNumber,
                    JoinSpan = dmJoinMap.InputCurrentResolution.JoinSpan
                },
                new JoinMetadata()
                {
                    Description = "Gets the current resolution of the tx hdmi input",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial
                });
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