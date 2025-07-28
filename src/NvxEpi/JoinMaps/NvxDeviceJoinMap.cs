using PepperDash.Essentials.Core;

namespace NvxEpi.JoinMaps;

public class NvxDeviceJoinMap : JoinMapBaseAdvanced
{
    [JoinName("AudioInput")] public JoinDataComplete AudioInput = new(
        new JoinData
            {
                JoinNumber = 4,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Audio Input Source"
            });

    [JoinName("AudioRoute")] public JoinDataComplete AudioRoute = new(
        new JoinData
            {
                JoinNumber = 2,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Audio Route"
            });

    [JoinName("AudioRouteString")]
    public JoinDataComplete AudioRouteString = new(
        new JoinData
        {
            JoinNumber = 2,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Audio Route String"
        });

    [JoinName("DeviceName")] public JoinDataComplete DeviceName = new(
        new JoinData
            {
                JoinNumber = 11,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Device Name"
            });

    [JoinName("DeviceOnline")] public JoinDataComplete DeviceOnline = new(
        new JoinData
            {
                JoinNumber = 1,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Device Online"
            });

    [JoinName("Hdmi1Capability")] public JoinDataComplete Hdmi1Capability = new(
        new JoinData
            {
                JoinNumber = 6,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Hdmi1 Capability"
            });

    [JoinName("Hdmi1SyncDetected")] public JoinDataComplete Hdmi1SyncDetected = new(
        new JoinData
            {
                JoinNumber = 3,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi1 Sync Detected"
            });

    [JoinName("Hdmi2Capability")] public JoinDataComplete Hdmi2Capability = new(
        new JoinData
            {
                JoinNumber = 7,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Hdmi2 Capability"
            });

    [JoinName("Hdmi2SyncDetected")] public JoinDataComplete Hdmi2SyncDetected = new(
        new JoinData
            {
                JoinNumber = 4,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi2 Sync Detected"
            });

    [JoinName("HdmiIn1Present")] public JoinDataComplete HdmiIn1Present = new(
        new JoinData
            {
                JoinNumber = 6,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi In1 Present"
            });

    [JoinName("HdmiIn2Present")] public JoinDataComplete HdmiIn2Present = new(
        new JoinData
            {
                JoinNumber = 7,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi In2 Present"
            });

    [JoinName("HdmiOutputDisableByHdcp")] public JoinDataComplete HdmiOutputDisableByHdcp = new            (
        new JoinData
            {
                JoinNumber = 8,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi Output Disabled By Hdcp"
            });

    [JoinName("HdmiOutputResolution")] public JoinDataComplete HdmiOutputResolution = new(
        new JoinData
            {
                JoinNumber = 8,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Hdmi Output Resolution"
            });

    [JoinName("MulticastAudioAddress")] public JoinDataComplete MulticastAudioAddress = new(
        new JoinData
            {
                JoinNumber = 16,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Secondary Audio Address"
            });

    [JoinName("MulticastVideoAddress")] public JoinDataComplete MulticastVideoAddress = new(
        new JoinData
            {
                JoinNumber = 15,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Multicast Video Address"
            });

    [JoinName("NaxInput")] public JoinDataComplete NaxInput = new(
        new JoinData
            {
                JoinNumber = 13,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Nax Audio Input Source"
            });

    [JoinName("NaxRoute")] public JoinDataComplete NaxRoute = new(
        new JoinData
            {
                JoinNumber = 12,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Nax Route"
            });

    [JoinName("NaxRxAddress")] public JoinDataComplete NaxRxAddress = new(
        new JoinData
            {
                JoinNumber = 18,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "NaxRxAddress"
            });

    [JoinName("NaxTxAddress")] public JoinDataComplete NaxTxAddress = new(
        new JoinData
            {
                JoinNumber = 17,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "NaxTxAddress"
            });

    [JoinName("StreamStarted")] public JoinDataComplete StreamStarted = new(
        new JoinData
            {
                JoinNumber = 2,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Stream Started"
            });

    [JoinName("StreamUrl")] public JoinDataComplete StreamUrl = new(
        new JoinData
            {
                JoinNumber = 14,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Stream Url"
            });

    [JoinName("DanteInput")] public JoinDataComplete DanteInput = new(
        new JoinData
        {
            JoinNumber = 10,
            JoinSpan = 1,
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
            JoinType = eJoinType.AnalogSerial,
            Description = "Dante Input"
        });

    [JoinName("SupportsNax")] public JoinDataComplete SupportsNax = new(
        new JoinData
            {
                JoinNumber = 12,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "SupportsNax"
            });

    [JoinName("SupportsVideowall")] public JoinDataComplete SupportsVideowall = new(
        new JoinData
            {
                JoinNumber = 9,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Supports Videowall"
            });

    [JoinName("UsbRoute")] public JoinDataComplete UsbRoute = new(
        new JoinData
            {
                JoinNumber = 5,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Usb Route"
            });

    [JoinName("VideoInput")] public JoinDataComplete VideoInput = new(
        new JoinData
            {
                JoinNumber = 3,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Video Input Source"
            });

    [JoinName("VideoRoute")] public JoinDataComplete VideoRoute = new(
        new JoinData
            {
                JoinNumber = 1,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Video Route"
            });

    [JoinName("VideoRouteString")]
    public JoinDataComplete VideoRouteString = new(
        new JoinData
        {
            JoinNumber = 1,
            JoinSpan = 1
        },
        new JoinMetadata
        {
            JoinCapabilities = eJoinCapabilities.ToSIMPL,
            JoinType = eJoinType.Serial,
            Description = "Video Route String"
        });

    [JoinName("VideowallMode")] public JoinDataComplete VideowallMode = new(
        new JoinData
            {
                JoinNumber = 9,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Videowall Mode"
            });

    [JoinName("VideoAspectRatioMode")] public JoinDataComplete VideoAspectRatioMode = new(
        new JoinData
            {
                JoinNumber = 11,
                JoinSpan = 1,
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Video Aspect Ratio Mode"
            });

    [JoinName("PortCount")]
    public JoinDataComplete PortCount = new(
        new JoinData
            {
                JoinNumber = 20,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Port Count"
            });

    [JoinName("PortIndex")]
    public JoinDataComplete PortIndex = new(
        new JoinData
            {
                JoinNumber = 21,
                JoinSpan = 5
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Network Port Index"
            });

    [JoinName("PortName")]
    public JoinDataComplete PortName = new(
        new JoinData
            {
                JoinNumber = 31,
                JoinSpan = 5
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Network Port Name"
            });

    [JoinName("PortDescription")]
    public JoinDataComplete PortDescription = new(
        new JoinData
            {
                JoinNumber = 41,
                JoinSpan = 5
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Network Port Description"
            });
    
    [JoinName("PortVlanName")]
    public JoinDataComplete PortVlanName = new(
        new JoinData
            {
                JoinNumber = 51,
                JoinSpan = 5
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Network Port VLAN Name"
            });

    [JoinName("PortIpManagementAddress")]
    public JoinDataComplete PortIpManagementAddress = new(  
        new JoinData
            {
                JoinNumber = 61,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Network Port IP Management Address"
            });

    [JoinName("PortSystemName")]
    public JoinDataComplete PortSystemName = new(
        new JoinData
            {
                JoinNumber = 71,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Network Port System Name"
            });
    
    [JoinName("PortSystemNameDescription")]
    public JoinDataComplete PortSystemNameDescription = new(
        new JoinData
            {
                JoinNumber = 81,
                JoinSpan = 1
            },
        new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Network Port System Name Description"
            });

    public NvxDeviceJoinMap(uint joinStart)
        : base(joinStart, typeof(NvxDeviceJoinMap)) { }
}