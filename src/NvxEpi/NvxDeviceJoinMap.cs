using PepperDash.Essentials.Core;

namespace NvxEpi
{
    public class NvxDeviceJoinMap(uint joinStart) : JoinMapBaseAdvanced(joinStart, typeof(NvxDeviceJoinMap))
    {
        [JoinName("DeviceName")]
        public JoinDataComplete DeviceName = new(
            new JoinData
            {
                JoinNumber = 1,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Device Name"
            });

        [JoinName("DeviceOnline")]
        public JoinDataComplete DeviceOnline = new(
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

        [JoinName("VideoInput")]
        public JoinDataComplete VideoInput = new(
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

        [JoinName("AudioInput")]
        public JoinDataComplete AudioInput = new(
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

        [JoinName("NaxTxInput")]
        public JoinDataComplete NaxTxInput = new(
            new JoinData
            {
                JoinNumber = 5,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Nax Audio Transmit Source"
            });

        [JoinName("DanteTxInput")]
        public JoinDataComplete DanteTxInput = new(
            new JoinData
            {
                JoinNumber = 6,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.AnalogSerial,
                Description = "Dante Transmit Input"
            });

        [JoinName("Hdmi1Capability")]
        public JoinDataComplete Hdmi1Capability = new(
            new JoinData
            {
                JoinNumber = 11,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Hdmi1 Capability"
            });

        [JoinName("Hdmi1SyncDetected")]
        public JoinDataComplete Hdmi1SyncDetected = new(
            new JoinData
            {
                JoinNumber = 11,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi1 Sync Detected"
            });

        [JoinName("Hdmi1Name")]
        public JoinDataComplete Hdmi1Name = new(
            new JoinData
            {
                JoinNumber = 11,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Hdmi1 Name"
            });

        [JoinName("Hdmi2Capability")]
        public JoinDataComplete Hdmi2Capability = new(
            new JoinData
            {
                JoinNumber = 12,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Hdmi2 Capability"
            });

        [JoinName("Hdmi2SyncDetected")]
        public JoinDataComplete Hdmi2SyncDetected = new(
            new JoinData
            {
                JoinNumber = 12,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Hdmi2 Sync Detected"
            });

        [JoinName("Hdmi2Name")]
        public JoinDataComplete Hdmi2Name = new(
            new JoinData
            {
                JoinNumber = 12,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Hdmi2 Name"
            });

        [JoinName("Usbc1Capability")]
        public JoinDataComplete Usbc1Capability = new(
            new JoinData
            {
                JoinNumber = 13,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Usbc1 Capability"
            });

        [JoinName("Usbc1SyncDetected")]
        public JoinDataComplete Usbc1SyncDetected = new(
            new JoinData
            {
                JoinNumber = 13,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Usbc1 Sync Detected"
            });

        [JoinName("Usbc1Name")]
        public JoinDataComplete Usbc1Name = new(
            new JoinData
            {
                JoinNumber = 13,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Usbc1 Name"
            });

        [JoinName("Usbc2Capability")]
        public JoinDataComplete Usbc2Capability = new(
            new JoinData
            {
                JoinNumber = 14,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Analog,
                Description = "Usbc2 Capability"
            });

        [JoinName("Usbc2SyncDetected")]
        public JoinDataComplete Usbc2SyncDetected = new(
            new JoinData
            {
                JoinNumber = 14,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Digital,
                Description = "Usbc2 Sync Detected"
            });

        [JoinName("Usbc2Name")]
        public JoinDataComplete Usbc2Name = new(
            new JoinData
            {
                JoinNumber = 14,
                JoinSpan = 1
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Usbc1 Name"
            });

        [JoinName("StreamUrl")]
        public JoinDataComplete StreamUrl = new(
            new JoinData
            {
                JoinNumber = 21,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Stream Url"
            });

        [JoinName("MulticastVideoAddress")]
        public JoinDataComplete MulticastVideoAddress = new(
            new JoinData
            {
                JoinNumber = 22,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial,
                Description = "Multicast Video Address"
            });

        [JoinName("NaxTxAddress")]
        public JoinDataComplete NaxTxAddress = new(
            new JoinData
            {
                JoinNumber = 23,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "NaxTxAddress"
            });

        [JoinName("NaxRxAddress")]
        public JoinDataComplete NaxRxAddress = new(
            new JoinData
            {
                JoinNumber = 24,
                JoinSpan = 1,
            },
            new JoinMetadata
            {
                JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                JoinType = eJoinType.Serial,
                Description = "NaxRxAddress"
            });

        /*
        [JoinName("PortCount")]
        public JoinDataComplete PortCount = new(
            new JoinData
                {
                    JoinNumber = 30,
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
                    JoinNumber = 31,
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
                    JoinNumber = 36,
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
                    JoinNumber = 41,
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
                    JoinNumber = 46,
                    JoinSpan = 5
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
                    JoinNumber = 51,
                    JoinSpan = 5
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
                    JoinNumber = 56,
                    JoinSpan = 5
                },
            new JoinMetadata
                {
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Serial,
                    Description = "Network Port System Name Description"
                });
                */
    }
}