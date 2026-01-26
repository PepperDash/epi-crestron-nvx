# Crestron NVX EPI - Configuration Guide

The NVX plugin provides device control and routing over Crestron NVX type devices without the need for an XIO director.

<!-- START Minimum Essentials Framework Versions -->
### Minimum Essentials Framework Versions

- 2.7.4
<!-- END Minimum Essentials Framework Versions -->

### ⚠️ IMPORTANT: Device Naming

The `name` property in the Essentials device config is what the actual NVX device will be named. **This value must not contain any spaces or special characters.** Use only alphanumeric characters and hyphens.

<!-- START Supported Types -->
### Supported Types

**Transmitters & Receivers:**
- DmNvx350, 350C, 351, 351C, 352, 352C (35x Series)
- DmNvx360, 360C, 363, 363C, E760, E760C (36x Series)  
- DmNvx384, 384C, 385, 385C (38x Series - 5K with 4x1 switcher)
- DmNvxD30, D30C (D3x - RX only)
- DmNvxE30, E30C, E31, E31C (E3x - TX only)

**Controllers:**
- XioDirector, 80, 160

**Testing:**
- MockNvxDevice
<!-- END Supported Types -->

<!-- START Config Example -->
### Config Example

#### TX - 35x/36x Series (Same Config Pattern)

```json
{
  "key": "nvx-tx-1",
  "uid": 1,
  "name": "ConferenceRoom-TX",
  "type": "DmNvx363",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x40"
    },
    "mode": "tx",
    "deviceId": 1,
    "multicastVideoAddress": "239.0.0.2",
    "multicastAudioAddress": "239.0.0.3",
    "defaultVideoInput": "Hdmi1",
    "defaultAudioInput": "Hdmi1",
    "enableAutoRoute": true
  }
}
```
**Note:** Config identical for DmNvx351, 352, 360, 363, E760. Only change `type` field.

#### TX - 38x Series (Supports USB-C as Video Source)

```json
{
  "key": "nvx-38x-tx",
  "uid": 2,
  "name": "AdvancedRoom-TX",
  "type": "DmNvx385",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x41"
    },
    "mode": "tx",
    "deviceId": 1,
    "multicastVideoAddress": "239.0.0.4",
    "multicastAudioAddress": "239.0.0.5",
    "defaultVideoInput": "Usbc1",
    "defaultAudioInput": "Usbc1"
  }
}
```
**Note:** DmNvx384/385 only. USB-C video inputs (Usbc1/Usbc2). Otherwise, config same as 35x/36x.

#### RX - 35x/36x/38x Series (Same Config Pattern)

```json
{
  "key": "nvx-rx-1",
  "uid": 50,
  "name": "Display-RX",
  "type": "DmNvx363",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x50"
    },
    "mode": "rx",
    "deviceId": 1
  }
}
```
**Note:** Config identical for ALL RX devices (35x, 36x, 38x, D3x). Only change `type` field.

#### RX with USB

```json
{
  "key": "nvx-rx-usb",
  "uid": 51,
  "name": "Display-USB-RX",
  "type": "DmNvx363",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x51"
    },
    "mode": "rx",
    "deviceId": 2,
    "usb": {
      "mode": "remote",
      "default": "nvx-encoder-1",
      "followVideo": true
    }
  }
}
```
**Note:** Add USB block to any RX. Works for 35x, 36x, 38x, D3x.

#### TX with Local USB

```json
{
  "key": "nvx-tx-usb",
  "uid": 3,
  "name": "Laptop-TX",
  "type": "DmNvx363",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x42"
    },
    "mode": "tx",
    "deviceId": 2,
    "multicastVideoAddress": "239.0.0.6",
    "multicastAudioAddress": "239.0.0.7",
    "usb": {
      "mode": "local",
      "default": "nvx-decoder-1",
      "followVideo": true
    }
  }
}
```
**Note:** Add USB block to any TX. Works for 35x, 36x, 38x.

#### E3x (TX Only, Simple)

```json
{
  "key": "nvx-e3x-tx",
  "uid": 100,
  "name": "Source-E3x",
  "type": "DmNvxE30",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x60"
    },
    "mode": "tx",
    "deviceId": 1,
    "multicastVideoAddress": "239.0.0.8",
    "multicastAudioAddress": "239.0.0.9"
  }
}
```
**Note:** E3x is TX-only. Config identical to 35x/36x TX (no USB-C). Same for DmNvxE31.

#### D3x (RX Only, Simple)

```json
{
  "key": "nvx-d3x-rx",
  "uid": 200,
  "name": "Display-D3x",
  "type": "DmNvxD30",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x70"
    },
    "mode": "rx",
    "deviceId": 1
  }
}
```
**Note:** D3x is RX-only. Config identical to any RX. Same for DmNvxD30C.

#### XIO Director

```json
{
  "key": "nvx-director1",
  "uid": 300,
  "name": "nvx-Director",
  "type": "XioDirector",
  "group": "nvx",
  "properties": {
    "control": {
      "method": "ipid",
      "ipid": "0x80"
    }
  }
}
```
**Note:** XioDirector, XioDirector80, XioDirector160 use same config. Only change `type`.

#### TX with XIO Director (Multi-Domain)

```json
{
  "key": "nvx-tx-xio",
  "uid": 5,
  "name": "Domain-1-TX",
  "type": "DmNvx360c",
  "group": "nvx",
  "properties": {
    "mode": "tx",
    "deviceId": 11,
    "ParentDeviceKey": "director1",
    "DomainId": 11,
    "usb": {
      "mode": "local"
    }
  }
}
```
**Note:** Add `ParentDeviceKey` and `DomainId` when using XIO Director. These are required for any Director-based setup, regardless of single or multi-domain.

#### Mock TX

```json
{
  "key": "mock-tx",
  "uid": 400,
  "name": "Mock-TX",
  "type": "MockNvxDevice",
  "group": "nvx",
  "properties": {
    "mode": "tx",
    "deviceId": 1,
    "IncludeInMatrixRouting": true
  }
}
```
**Note:** Testing/development only. No multicast required for Mock.

#### Mock RX

```json
{
  "key": "mock-rx",
  "uid": 401,
  "name": "Mock-RX",
  "type": "MockNvxDevice",
  "group": "nvx",
  "properties": {
    "mode": "rx",
    "deviceId": 1,
    "IncludeInMatrixRouting": true
  }
}
```
**Note:** Testing/development only. No control/ipid required for Mock.
<!-- END Config Example -->

<!-- START Device Series Capabilities -->
### Device Series Capabilities

| Series | Inputs | Audio | USB | Special |
|--------|--------|-------|-----|---------|
| 35x | 2x HDMI | Pass-through | Yes | Basic |
| 36x | 2x HDMI | Volume control | Yes | Audio volume |
| 38x (384/385) | 2x HDMI + 2x USB-C | 7.1 surround | Yes | 5K, 4x1 switcher, (385: downmixing) |
| D3x | - | Secondary audio | - | RX only |
| E3x | - | Secondary audio | - | TX only |
<!-- END Device Series Capabilities -->

<!-- START Core Properties -->
### Core Properties

| Property | Type | TX | RX | Description |
|----------|------|----|----|-------------|
| `key` | string | ✓ | ✓ | Unique identifier |
| `uid` | integer | ✓ | ✓ | Essentials UID |
| `name` | string | ✓ | ✓ | Device name (alphanumeric/hyphens only) |
| `type` | string | ✓ | ✓ | Device model (DmNvx351, DmNvx363, etc.) |
| `group` | string | ✓ | ✓ | Always "nvx" |
| `control.method` | string | ✓ | ✓ | Always "ipid" |
| `control.ipid` | string | ✓ | ✓ | Hex IPID (0x00-0xFF) |
| `mode` | string | ✓ | ✓ | "tx" or "rx" |
| `deviceId` | integer | ✓ | ✓ | Unique ID 1-99 (routing identifier) |
| `ParentDeviceKey` | string | ✗ | ✗ | XIO Director key (required when using Director) |
| `DomainId` | integer | ✗ | ✗ | Domain ID (required when using XIO Director) |
<!-- END Core Properties -->

### Property Details

**Core Configuration Properties:**

- **`key`:** Unique identifier within the system. Used for referencing in routing rules.

- **`uid`:** Essentials system UID. Must be unique across all devices in the system.

- **`name`:** Device name (alphanumeric and hyphens only, no spaces). This becomes the actual NVX device name. **CRITICAL: Do not use spaces or special characters.**

- **`type`:** Device model number (e.g., DmNvx351, DmNvx363, DmNvx385). Case-insensitive.

- **`group`:** Always set to `"nvx"` for all NVX devices.

- **`control.method`:** Always `"ipid"` for NVX devices.

- **`control.ipid`:** Hex IPID address (0x00-0xFF). Must be unique per device and documented.

- **`mode`:** `"tx"` (transmitter) or `"rx"` (receiver). **Critical:** Many defaults are set based on this value. Changing via web interface is not supported.

- **`deviceId`:** Virtual device ID (1-99) used for routing. When routing to a receiver, send the transmitter's `deviceId` to the receiver's Video/Audio Route join. Example: route TX-1 (deviceId=1) to RX-1 by sending `1` to RX-1's Video Route analog join.

- **`ParentDeviceKey`:** (XIO Director only) Key of the XIO Director managing this device. Required when any devices use an XIO Director for routing. When present, `DomainId` must also be set.

- **`DomainId`:** (XIO Director only) Numeric domain ID assigned to this device on the XIO Director. Required whenever `ParentDeviceKey` is present. All devices in the same domain share the same `DomainId`.

**TX-Only Properties:**

- **`multicastVideoAddress`:** Video stream multicast address. **Must end in even octet** (e.g., 239.0.0.2, 239.0.0.4). Use locally scoped range (239.0.0.0 - 239.255.255.255). Reference: [IP Multicast Addressing](http://www.tcpipguide.com/free/t_IPMulticastAddressing.htm)

- **`multicastAudioAddress`:** Audio/secondary audio multicast. Typically set to video address + 1 (e.g., 239.0.0.3).

- **`defaultVideoInput`:** Initial video source. Supported: Hdmi1, Hdmi2, Stream, Usbc1, Usbc2 (38x only)

- **`defaultAudioInput`:** Initial audio source. Supported: Hdmi1, Hdmi2, AnalogAudio, PrimaryStream, SecondaryStream, Dante, Usbc1, Usbc2, Nax

- **`enableAutoRoute`:** Enable automatic HDMI input switching when signal detected.

**USB Routing (Both Modes):**

- **`usb.mode`:** `"local"` (device provides USB) or `"remote"` (device uses external USB device)

- **`usb.default`:** Device key for USB routing target

- **`usb.followVideo`:** `true` = USB follows video routing, `false` = independent USB routing

<!-- START Join Maps -->
### Join Maps

#### Digitals

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | R | Device Online |
| 2 | R | Stream Started |
| 3 | R | HDMI 1 Sync Detected |
| 4 | R | HDMI 2 Sync Detected |
| 5 | R | USB Route Changed |
| 6 | R | Supports HDMI 1 |
| 7 | R | Supports HDMI 2 |
| 8 | R | Output Disabled by HDCP |
| 9 | R | Supports Videowall |
| 12 | R | Supports NAX |

#### Analogs

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | R | Video Route |
| 2 | R | Audio Route |
| 3 | R | Video Input |
| 4 | R | Audio Input |
| 5 | R | USB Route |
| 6 | R | HDMI 1 HDCP Capability |
| 7 | R | HDMI 2 HDCP Capability |
| 11 | R | NAX Input |
| 13 | R | NAX Input (secondary) |

#### Serials

| Join | Direction | Description |
|------|-----------|-------------|
| 1 | R | Video Route |
| 2 | R | Audio Route |
| 3 | R | Video Input |
| 4 | R | Audio Input |
| 5 | R | USB Route |
| 6 | R | HDMI 1 HDCP Capability |
| 7 | R | HDMI 2 HDCP Capability |
| 10 | R | Dante Input |
| 11 | R | Device Name |
| 12 | R | NAX Route |
| 13 | R | NAX Input |
| 14 | R | Stream URL |
| 15 | R | Multicast Video Address |
| 16 | R | Secondary Audio Address |
| 17 | R | NAX TX Address |
| 18 | R | NAX RX Address |
<!-- END Join Maps -->

### Join Details

**Digital Joins:**
- **1 (Device Online):** Device status - online/offline
- **2 (Stream Started):** Stream is actively transmitting/receiving
- **3-4 (HDMI Sync):** Detects valid signal on HDMI inputs
- **5 (USB Route Changed):** USB routing has changed
- **6-7 (Supports HDMI):** Reports HDMI input availability
- **8 (Output Disabled by HDCP):** Output blocked due to HDCP protection
- **9 (Supports Videowall):** Device supports videowall mode
- **12 (Supports NAX):** Device supports NAX audio

**Analog Joins - Video/Audio Routing:**
- **1 (Video Route):** Select video source by device ID (RX mode only)
- **2 (Audio Route):** Select audio source by device ID (RX mode only)
- **3 (Video Input):** Current video input: Disabled=0, Hdmi1=1, Hdmi2=2, Stream=3, Usbc1=4, Usbc2=5
- **4 (Audio Input):** Current audio input: Automatic=0, Input1=1, Input2=2, AnalogAudio=3, PrimaryStream=4, SecondaryStream=5, Dante=6, Usbc1=7, Usbc2=8, Nax=9
- **5 (USB Route):** USB device routing selection
- **6-7 (HDCP Capability):** HDCP levels: HDCPOff=0, HDCPAuto=1, HDCP1x=2, HDCP2x=3

**Serial Joins:**
- **1-5:** Text representation of Video/Audio/USB routes
- **10 (Dante Input):** Dante audio input address
- **11 (Device Name):** Device name string
- **12-13 (NAX Route/Input):** NAX audio routing and input
- **14 (Stream URL):** RTP stream URL
- **15 (Multicast Video Address):** Video multicast address
- **16 (Secondary Audio Address):** Secondary audio multicast address
- **17-18 (NAX Addresses):** NAX TX/RX addresses

<!-- START Interfaces Implemented -->
### Interfaces Implemented

- IStream
- ISecondaryAudioStream
- IRoutingWithFeedback
- IMatrixRouting
- ICommunicationMonitor
- IBridgeAdvanced
- IHasFeedback
- IUsbStreamWithHardware (TX with USB)
- IBasicVolumeWithFeedback (36x, 38x)
- IUsbcInput (38x only)
- IMultiview (38x only)
<!-- END Interfaces Implemented -->

<!-- START Base Classes -->
### Base Classes

- ReconfigurableDevice
- EssentialsDevice
- JoinMapBaseAdvanced
- MessengerBase
<!-- END Base Classes -->

<!-- START Routing Framework -->
### Routing Framework

**NvxGlobalRouter** (automatic, not configured):
- PrimaryStreamRouter: Video/audio stream routing
- SecondaryAudioRouter: NAX audio routing
- UsbRouter: USB device routing

Integration with Essentials matrix routing framework:
- TX devices register as routing inputs
- RX devices register as routing outputs
- Mock devices participate via `includeInMatrixRouting: true`
<!-- END Routing Framework -->

<!-- START Configuration Best Practices -->
### Configuration Best Practices

**Device Naming:**
- Alphanumeric and hyphens only (no spaces)
- Format: `RoomName-DeviceType-Mode` (e.g., `ConferenceRoom-MainDisplay-RX`)

**Device IDs:**
- Sequential per mode (TX: 1,2,3... RX: 1,2,3...)
- Each ID must be unique within its mode group

**Multicast Addressing:**
- Video: even octets (239.0.0.2, 239.0.0.4, etc.)
- Audio: +1 from video (239.0.0.3, 239.0.0.5, etc.)
- Use unique subnets per zone (239.x.y.z)

**IPID Assignment:**
- Reserve block for NVX: 0x40-0x5F
- Ensure no conflicts with other Crestron devices
- Document all assignments

**USB Routing:**
- Local mode: device provides USB encoder/decoder
- Remote mode: device uses external USB device
- `followVideo: true` for integrated setups
- `followVideo: false` for independent routing

**Validation:**
- All names valid (alphanumeric/hyphens)
- UIDs unique system-wide
- Device IDs unique within TX and RX groups
- IPIDs unique and documented
- Multicast video = even, audio = video + 1
- Test communication before deployment
<!-- END Configuration Best Practices -->

