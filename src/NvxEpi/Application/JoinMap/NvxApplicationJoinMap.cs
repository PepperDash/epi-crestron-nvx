using PepperDash.Essentials.Core;

namespace NvxEpi.Application.JoinMap
{
    public class NvxApplicationJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("EnableAudioBreakaway")] public JoinDataComplete EnableAudioBreakaway = new JoinDataComplete(
            new JoinData {JoinNumber = 4, JoinSpan = 1},
            new JoinMetadata
                {
                    Description = "Enable audio breakaway routing",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

        [JoinName("EnableUsbBreakaway")] public JoinDataComplete EnableUsbBreakaway = new JoinDataComplete(
            new JoinData {JoinNumber = 5, JoinSpan = 1},
            new JoinMetadata
                {
                    Description = "Enable USB breakaway routing",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

        //Video Transmitters
        [JoinName("HdcpSupportCapability")] public JoinDataComplete HdcpSupportCapability =
            new JoinDataComplete(new JoinData {JoinNumber = 1201, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Input HDCP Support Capability",
                        JoinCapabilities = eJoinCapabilities.FromSIMPL,
                        JoinType = eJoinType.Analog
                    });


        [JoinName("InputCurrentResolution")] public JoinDataComplete InputCurrentResolution =
            new JoinDataComplete(new JoinData {JoinNumber = 2401, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "DM Chassis Input Current Resolution",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Serial
                    });

        [JoinName("InputEndpointOnline")] public JoinDataComplete InputEndpointOnline =
            new JoinDataComplete(new JoinData {JoinNumber = 501, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Input Endpoint Online",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Digital
                    });

        [JoinName("InputNames")] public JoinDataComplete InputNames =
            new JoinDataComplete(new JoinData {JoinNumber = 101, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Input Name",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Serial
                    });

        [JoinName("InputVideoNames")] public JoinDataComplete InputVideoNames =
            new JoinDataComplete(new JoinData {JoinNumber = 501, JoinSpan = 200},
                new JoinMetadata
                    {
                        Description = "Video Input Name",
                        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                        JoinType = eJoinType.Serial
                    });

        [JoinName("TxAdvancedIsPresent")] public JoinDataComplete TxAdvancedIsPresent =
            new JoinDataComplete(new JoinData {JoinNumber = 1001, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Tx Advanced Is Present",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Digital
                    });

        [JoinName("VideoSyncStatus")] public JoinDataComplete VideoSyncStatus =
            new JoinDataComplete(new JoinData {JoinNumber = 101, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Input Video Sync",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Digital
                    });


        //[JoinName("InputAudioNames")] public JoinDataComplete InputAudioNames;
        //[JoinName("OutputAudio")] public JoinDataComplete OutputAudio;
        //[JoinName("OutputAudioNames")] public JoinDataComplete OutputAudioNames;
        //[JoinName("OutputCurrentAudioInputNames")] public JoinDataComplete OutputCurrentAudioInputNames;

        [JoinName("OutputCurrentVideoInputNames")] public JoinDataComplete OutputCurrentVideoInputNames =
            new JoinDataComplete(new JoinData {JoinNumber = 2001, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Video Output Currently Routed Video Input Name",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Serial
                    });


        [JoinName("OutputDisabledByHdcp")] public JoinDataComplete OutputDisabledByHdcp =
            new JoinDataComplete(new JoinData {JoinNumber = 1201, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Output Disabled by HDCP",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Digital
                    });

        [JoinName("OutputEndpointOnline")] public JoinDataComplete OutputEndpointOnline =
            new JoinDataComplete(new JoinData {JoinNumber = 701, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Output Endpoint Online",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Digital
                    });

        [JoinName("OutputHorizontalResolution")] public JoinDataComplete OutputHorizontalResolution = new JoinDataComplete
            (
            new JoinData
                {
                    JoinNumber = 3301,
                    JoinSpan = 32
                },
            new JoinMetadata
                {
                    Description = "Analog value of horizontal resolution on HDMI output",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Analog
                });

        [JoinName("OutputEdidManufacturer")] public JoinDataComplete OutputEdidManufacturer = new JoinDataComplete
            (
            new JoinData
                {
                    JoinNumber = 3501,
                    JoinSpan = 32
                },
            new JoinMetadata
                {
                    Description = "Analog value of horizontal resolution on HDMI output",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Analog
                });

        [JoinName("OutputNames")] public JoinDataComplete OutputNames =
            new JoinDataComplete(new JoinData {JoinNumber = 301, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Output Name",
                        JoinCapabilities = eJoinCapabilities.ToSIMPL,
                        JoinType = eJoinType.Serial
                    });

        //[JoinName("OutputUsb")] public JoinDataComplete OutputUsb;

        [JoinName("OutputVideo")] public JoinDataComplete OutputVideo =
            new JoinDataComplete(new JoinData {JoinNumber = 101, JoinSpan = 32},
                new JoinMetadata
                    {
                        Description = "Output Video Set / Get",
                        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                        JoinType = eJoinType.Analog
                    });

        [JoinName("OutputVideoNames")] public JoinDataComplete OutputVideoNames =
            new JoinDataComplete(new JoinData {JoinNumber = 901, JoinSpan = 200},
                new JoinMetadata
                    {
                        Description = "Video Input Name",
                        JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                        JoinType = eJoinType.Serial
                    });

        [JoinName("ReceiverSerialPorts")] public JoinDataComplete ReceiverSerialPorts = new JoinDataComplete(
            new JoinData
                {
                    JoinNumber = 3901,
                    JoinSpan = 32
                },
            new JoinMetadata
                {
                    Description = "COM TX/RX for receivers if registered",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Serial
                });


        public NvxApplicationJoinMap(uint joinStart)
            : base(joinStart, typeof (NvxApplicationJoinMap)) { }
    }
}