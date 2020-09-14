using PepperDash.Essentials.Core;

namespace NvxEpi.Device.JoinMaps
{
    public class NvxDeviceJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("Device Online")]
        public JoinDataComplete DeviceOnline = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 1,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Device Online"
            });

        [JoinName("Stream Started")]
        public JoinDataComplete StreamStarted = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 2,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Stream Started"
            });

        [JoinName("Hdmi1 Sync Detected")]
        public JoinDataComplete Hdmi1SyncDetected = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 3,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi1 Sync Detected"
            });

        [JoinName("Hdmi2 Sync Detected")]
        public JoinDataComplete Hdmi2SyncDetected = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 4,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi2 Sync Detected"
            });

        [JoinName("Hdmi Output Disabled By Hdcp")]
        public JoinDataComplete HdmiOutputDisableByHdcp = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 5,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi Output Disabled By Hdcp"
            });

        [JoinName("Video Route")]
        public JoinDataComplete VideoRoute = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 1,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Video Route"
            });

        [JoinName("Audio Route")]
        public JoinDataComplete AudioRoute = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 2,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Audio Route"
            });

        [JoinName("Video Input")]
        public JoinDataComplete VideoInput = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 3,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Video Input Source"
            });

        [JoinName("Audio Source")]
        public JoinDataComplete AudioInput = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 4,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Audio Input Source"
            });

        [JoinName("Usb Route")]
        public JoinDataComplete UsbRoute = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 5,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Audio Input Source"
            });

        [JoinName("Hdmi1 Capability")]
        public JoinDataComplete Hdmi1Capability = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 6,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Hdmi1 Capability"
            });

        [JoinName("Hdmi2 Capability")]
        public JoinDataComplete Hdmi2Capability = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 7,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Hdmi2 Capability"
            });
        

        [JoinName("Device Name")]
        public JoinDataComplete HdmiOutputResolution = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 8,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Device Name"
            });

        [JoinName("Videowall Mode")]
        public JoinDataComplete VideowallMode = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 9,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Videowall Mode"
            }); 

        [JoinName("Secondary Audio Status")]
        public JoinDataComplete SecondaryAudioStatus = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 9,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Secondary Audio Status"
            });

        [JoinName("Nax Source")]
        public JoinDataComplete NaxInput = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 6,
                JoinSpan = 1
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Nax Audio Input Source"
            });

        [JoinName("Stream Url")]
        public JoinDataComplete StreamUrl = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 13,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Stream Url"
            });

        [JoinName("Multicast Video Address")]
        public JoinDataComplete MulticastVideoAddress = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 14,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Multicast Video Address"
            });

        [JoinName("Multicast Audio Address")]
        public JoinDataComplete MulticastAudioAddress = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 15,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Multicast Audio Address"
            });

        

        [JoinName("Usb Remote Id")]
        public JoinDataComplete UsbRemoteId = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 18,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Usb Remote Id"
            });

        [JoinName("Usb Local Id")]
        public JoinDataComplete UsbLocalId = new JoinDataComplete(
            new JoinData()
            {
                JoinNumber = 18,
                JoinSpan = 1,
            },
            new JoinMetadata()
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Usb Remote Id"
            });

        public NvxDeviceJoinMap(uint joinStart)
            : base(joinStart)
        {
            
        }
    }
}