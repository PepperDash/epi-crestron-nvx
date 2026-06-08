# PepperDash NVX Plugin

The NVX plugin provides device control and routing for Crestron NVX streaming devices within the PepperDash Essentials framework, without requiring an XIO Director.

## Essentials Version

Requires Essentials 2.36.2 or later.

## Supported Device Types

The `type` field in config is case-insensitive.

| Category | Types |
|----------|-------|
| Encoders / Transmitters | DmNvx350, DmNvx350C, DmNvx351, DmNvx351C, DmNvx352, DmNvx352C, DmNvx363, DmNvx363C, DmNvx384, DmNvx384C |
| E-Series Encoders | DmNvxE10, DmNvxE20, DmNvxE202G, DmNvxE30, DmNvxE30C, DmNvxE31, DmNvxE31C, DmNvxE760, DmNvxE760C |
| Decoders / Receivers | DmNvx360, DmNvx360C, DmNvxD30, DmNvxD30C |
| Testing | mockNvxDevice |

## Configuration

### Properties

| Property | Type | Description |
|----------|------|-------------|
| `control` | object | Connection method and IPID |
| `mode` | string | `"tx"` (transmitter) or `"rx"` (receiver). Required. |
| `deviceId` | int | Unique virtual routing slot number. Used to route sources to destinations. |
| `multicastAddress` | string | Multicast address for TX streaming. Must have an even last octet. |
| `streamUrl` | string | Initial stream URL for RX devices. |
| `dmNaxTransmitAddress` | string | DM-NAX audio transmit address (TX). |
| `dmNaxReceiveAddress` | string | DM-NAX audio receive address (RX). |

### Transmitter Example

```json
{
    "key": "nvx-tx-1",
    "name": "Laptop",
    "type": "DmNvx351",
    "group": "nvx",
    "properties": {
        "control": {
            "method": "ipid",
            "ipid": "41"
        },
        "mode": "tx",
        "deviceId": 1,
        "multicastAddress": "239.0.0.2"
    }
}
```

### Receiver Example

```json
{
    "key": "nvx-rx-1",
    "name": "Display1",
    "type": "DmNvx360",
    "group": "nvx",
    "properties": {
        "control": {
            "method": "ipid",
            "ipid": "51"
        },
        "mode": "rx",
        "deviceId": 1
    }
}
```

### E-Series Encoder Example

```json
{
    "key": "nvx-e30-1",
    "name": "WallPlateEncoder",
    "type": "DmNvxE30",
    "group": "nvx",
    "properties": {
        "control": {
            "method": "ipid",
            "ipid": "61"
        },
        "mode": "tx",
        "deviceId": 3,
        "multicastAddress": "239.0.0.6"
    }
}
```

### D-Series Decoder Example

```json
{
    "key": "nvx-d30-1",
    "name": "MonitorDecoder",
    "type": "DmNvxD30",
    "group": "nvx",
    "properties": {
        "control": {
            "method": "ipid",
            "ipid": "71"
        },
        "mode": "rx",
        "deviceId": 2
    }
}
```

### Transmitter with DM-NAX Audio

```json
{
    "key": "nvx-tx-2",
    "name": "ConferenceRoom",
    "type": "DmNvx363",
    "group": "nvx",
    "properties": {
        "control": {
            "method": "ipid",
            "ipid": "42"
        },
        "mode": "tx",
        "deviceId": 2,
        "multicastAddress": "239.0.0.4",
        "dmNaxTransmitAddress": "239.1.0.4"
    }
}
```

### Full System Example

```json
{
    "devices": [
        {
            "key": "nvx-tx-1",
            "name": "Laptop",
            "type": "DmNvx351",
            "group": "nvx",
            "properties": {
                "control": { "method": "ipid", "ipid": "41" },
                "mode": "tx",
                "deviceId": 1,
                "multicastAddress": "239.0.0.2"
            }
        },
        {
            "key": "nvx-tx-2",
            "name": "BYOD",
            "type": "DmNvx363",
            "group": "nvx",
            "properties": {
                "control": { "method": "ipid", "ipid": "42" },
                "mode": "tx",
                "deviceId": 2,
                "multicastAddress": "239.0.0.4",
                "dmNaxTransmitAddress": "239.1.0.4"
            }
        },
        {
            "key": "nvx-rx-1",
            "name": "MainDisplay",
            "type": "DmNvx360",
            "group": "nvx",
            "properties": {
                "control": { "method": "ipid", "ipid": "51" },
                "mode": "rx",
                "deviceId": 1,
                "dmNaxReceiveAddress": "239.1.0.4"
            }
        },
        {
            "key": "nvx-rx-2",
            "name": "SideDisplay",
            "type": "DmNvxD30",
            "group": "nvx",
            "properties": {
                "control": { "method": "ipid", "ipid": "52" },
                "mode": "rx",
                "deviceId": 2
            }
        }
    ]
}
```

## Bridge / Join Map

The plugin implements `IBridgeAdvanced` for SIMPL bridge integration via `LinkToApi()`. Each device exposes the following joins relative to its bridge join start.

### Digital Joins

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | To SIMPL | Device Online |
| 11 | To SIMPL | HDMI 1 Sync Detected |
| 12 | To SIMPL | HDMI 2 Sync Detected |
| 13 | To SIMPL | USB-C 1 Sync Detected |
| 14 | To SIMPL | USB-C 2 Sync Detected |

### Analog Joins

| Join | Direction | Description |
|------|-----------|-------------|
| 11 | To/From SIMPL | HDMI 1 HDCP Capability |
| 12 | To/From SIMPL | HDMI 2 HDCP Capability |
| 13 | To/From SIMPL | USB-C 1 HDCP Capability |
| 14 | To/From SIMPL | USB-C 2 HDCP Capability |
| 30 | To SIMPL | Network Port Count |
| 31-35 | To SIMPL | Network Port Index (5 ports) |

### Serial Joins

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | To/From SIMPL | Device Name |
| 3 | To/From SIMPL | Video Input Source |
| 4 | To/From SIMPL | Audio Input Source |
| 5 | To/From SIMPL | NAX Audio Transmit Source |
| 6 | To/From SIMPL | Dante Transmit Input |
| 21 | To/From SIMPL | Stream URL |
| 22 | To SIMPL | Multicast Video Address |
| 23 | To/From SIMPL | NAX TX Address |
| 24 | To/From SIMPL | NAX RX Address |
| 31-35 | To SIMPL | Network Port Name (5 ports) |
| 36-40 | To SIMPL | Network Port Description (5 ports) |
| 41-45 | To SIMPL | Network Port VLAN Name (5 ports) |
| 46-50 | To SIMPL | Network Port IP Management Address (5 ports) |
| 51-55 | To SIMPL | Network Port System Name (5 ports) |
| 56-60 | To SIMPL | Network Port System Name Description (5 ports) |

### Video Input Source Values

| Value | Source |
|-------|--------|
| 0 | Disable |
| 1 | HDMI 1 |
| 2 | HDMI 2 |
| 3 | Stream |
| 11 | USB-C 1 |
| 12 | USB-C 2 |

### Audio Input Source Values

| Value | Source |
|-------|--------|
| 0 | Automatic / No Audio |
| 1 | Input 1 |
| 2 | Input 2 |
| 3 | Analog Audio |
| 4 | Primary Stream Audio |
| 5 | DM NAX Audio |
| 6 | Dante / AES-67 |
| 7 | Bluetooth (BTS) |
| 11 | USB-C 1 |
| 12 | USB-C 2 |
| 21 | eARC |

### Bridge Config Example

To bridge an NVX device in your Essentials room config:

```json
{
    "key": "nvx-tx-1-bridge",
    "name": "NVX TX 1 Bridge",
    "type": "eiscApiAdvanced",
    "group": "api",
    "properties": {
        "control": {
            "method": "ipidTcp",
            "ipid": "B1",
            "tcpSshProperties": {
                "address": "127.0.0.2",
                "port": 0
            }
        },
        "devices": [
            {
                "deviceKey": "nvx-tx-1",
                "joinStart": 1
            }
        ]
    }
}
```

## NVX Router

The plugin automatically creates a singleton `nvxRouter` device (key: `"nvxRouter"`) that implements `IMatrixRouting`. All NVX devices self-register with the router during initialization. No separate router config is needed.

### Routing

To route TX device 1 to RX device 1, send `deviceId` value `1` to the Video Source input of the receiver. The router coordinates the stream URL assignment automatically.

### Routing Port Keys

These port key strings are used when referencing routing inputs/outputs:

| Key | Description |
|-----|-------------|
| `stream` | Primary network stream |
| `hdmi1` | HDMI input 1 |
| `hdmi2` | HDMI input 2 |
| `analogAudio` | Analog audio |
| `dante_aes67` | Dante/AES67 audio |
| `dmNax` | DM-NAX audio |
| `dm` | DM input (E-series) |
| `arc` | eARC |
| `bts` | Bluetooth audio |
| `usbCIn1` | USB-C input 1 |
| `usbCIn2` | USB-C input 2 |
| `loopOut` | Audio loop out |


<!-- START Minimum Essentials Framework Versions -->

<!-- END Minimum Essentials Framework Versions -->
<!-- START Config Example -->

<!-- END Config Example -->
<!-- START Supported Types -->

<!-- END Supported Types -->
<!-- START Join Maps -->

<!-- END Join Maps -->
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- INvxNetworkPortInformation
- IMatrixRouting
- IRoutingWithFeedback
- INvxDevice
- IHasFeedback
- ICommunicationMonitor
- IBridgeAdvanced
<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- EssentialsDevice
- MessengerBase
- DeviceStateMessageBase
<!-- END Base Classes -->
<!-- START Public Methods -->
### Public Methods

- public void Route(string inputSlotKey, string outputSlotKey, eRoutingSignalType type)
- public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
- public void SetIncomingStreamUrl(string streamUrl)
- public void SetIncomingDmNaxStreamAddress(string address)
- public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
- public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
- public void SetIncomingStreamUrl(string streamUrl)
- public void SetIncomingDmNaxStreamAddress(string address)
- public class NvxDeviceJoinMap(uint joinStart)
- public class HdmiInputState(string key, string hdcpCapability, string hdcpSupport, bool syncDetected, string currentResolution, int audioChannelCount, string audioFormat, string colorspaceMode, string hdrType)
- public class MockDeviceMessenger(string key, string path, NvxMockDevice device)
- public class HdmiOutputState(bool disabledByHdcp, string outputResolution, string edidManufacturer)
- public class SecondaryAudioStateMessage(bool isStreamingSecondaryAudio, string secondaryAudioStreamStatus, string secondaryAudioStreamUrl)
- public class SecondaryAudioUpdateMessage(bool isStreamingSecondaryAudio, string secondaryAudioStreamStatus, string secondaryAudioStreamUrl)
- public class StreamStateMessage(bool isStreamingVideo, string videoStreamStatus, string streamUrl, string multicastAddress, bool isTransmitter)
- public class StreamUpdateMessage(bool isStreamingVideo, string videoStreamStatus, string streamUrl, string multicastAddress, bool isTransmitter)
<!-- END Public Methods -->
<!-- START Bool Feedbacks -->
### Bool Feedbacks

- IsOnline
- IsOnline
- IsStreamingFeedback
- IsTransmittingDmNaxFeedback
- IsReceivingDmNaxFeedback
- IsStreamingFeedback
- IsTransmittingDmNaxFeedback
- IsReceivingDmNaxFeedback
- SyncDetected
- DisabledByHdcpFeedback
<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->
### Int Feedbacks

- AudioChannelCount
<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->
### String Feedbacks

- StreamUrlFeedback
- StreamStatusFeedback
- DmNaxTxAddressFeedback
- DmNaxTransmitStatusFeedback
- DmNaxReceiveStatusFeedback
- DmNaxRxAddressFeedback
- MulticastAddressFeedback
- StreamUrlFeedback
- StreamStatusFeedback
- DmNaxTxAddressFeedback
- DmNaxTransmitStatusFeedback
- DmNaxRxAddressFeedback
- DmNaxReceiveStatusFeedback
- MulticastAddressFeedback
- HdcpCapability
- AudioFormat
- ColorspaceMode
- HdrType
- OutputResolutionFeedback
- EdidManufacturerFeedback
<!-- END String Feedbacks -->
