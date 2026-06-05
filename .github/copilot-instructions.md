# NVX EPI Plugin - Architecture Guide

This is a PepperDash Essentials plugin (EPI) that provides device control and routing for Crestron NVX streaming endpoints. It runs on Crestron 4-Series processors.

## Project Structure

```
src/NvxEpi/
├── NvxDevice.cs              # Main device implementation (~1175 lines)
├── INvxDevice.cs             # Core device interface
├── NvxDeviceFactory.cs       # Factory pattern for device creation
├── NvxDeviceProperties.cs    # Configuration model (deserialized from JSON)
├── NvxDeviceJoinMap.cs       # SIMPL bridge join map
├── NvxRouter.cs              # Singleton matrix router
├── NvxXioDirector.cs         # XIO Director domain support
├── NvxMockDevice.cs          # Mock device for testing without hardware
├── NvxHdmiInputPort.cs       # HDMI input port with feedbacks
├── NvxHdmiOutputPort.cs      # HDMI output port with feedbacks
├── NvxNetworkPortInfo.cs     # LLDP network port information
└── McMessengers/             # AppServer REST/WebSocket messengers
    ├── EndpointInfoMessenger.cs
    ├── PrimaryStreamStatusMessenger.cs
    ├── SecondaryAudioStatusMessenger.cs
    ├── HdmiInputMessenger.cs
    ├── HdmiOutputMessenger.cs
    └── MockDeviceMessenger.cs
```

## Key Concepts

### Device Model

`NvxDevice` wraps a Crestron `DmNvxBaseClass` (the SDK object that talks to hardware) and layers on Essentials abstractions: feedbacks, routing ports, bridge integration, and messengers.

Every NVX device operates in one of two modes set by config:
- **Transmitter (TX)** - encodes local sources (HDMI, USB-C) and streams over the network
- **Receiver (RX)** - receives a network stream and outputs to a local display via HDMI

The mode is set via the `mode` property in config (`"tx"` or `"rx"`) and determines which routing ports are registered, which feedbacks are active, and how routing commands are handled.

### Device Lifecycle

1. Essentials reads the device config JSON and matches the `type` field to a factory
2. `NvxDeviceFactory.BuildDevice()` deserializes `NvxDeviceProperties` and instantiates the correct Crestron SDK class (e.g., `DmNvx351`, `DmNvxE30`)
3. `NvxDevice` constructor stores the config and SDK object
4. `Initialize()` registers the device with the Crestron control system, subscribes to hardware events, and self-registers with the `NvxRouter` singleton

### Feedback Pattern

The plugin uses three feedback types from PepperDash Essentials Core:
- `BoolFeedback` - digital state (e.g., device online, sync detected)
- `IntFeedback` - analog values (e.g., HDCP capability level)
- `StringFeedback` - serial values (e.g., stream URL, multicast address)

Feedbacks are lazy-evaluated via lambdas:
```csharp
StreamUrlFeedback = new StringFeedback("StreamUrl", () => device.Control.ServerUrlFeedback.StringValue);
```

They update when you call `FireUpdate()` on them, typically in response to a hardware `OutputChange` event. They're collected in a `FeedbackCollection<Feedback>` on the device.

### Routing Architecture

`NvxRouter` is a singleton matrix router (key: `"nvxRouter"`) that automatically registers itself with the Essentials `DeviceManager`. It implements `IMatrixRouting`.

Each NVX device self-registers as either a transmitter or receiver slot. Routing works by:
1. An external system calls `NvxRouter.Route(inputSlot, outputSlot, signalType)`
2. For **video**: the receiver's stream URL is set to match the transmitter's multicast address
3. For **audio** (DM-NAX): the receiver's NAX address is set to match the transmitter's NAX address

Route feedback is event-driven: when a receiver's `StreamUrlFeedback` changes, the router scans transmitters for a matching URL to determine the current route.

### Routing Port Keys

These string constants identify routing input/output ports on devices:

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

### Input Selectors

Video and audio input sources are set via enums:

**NvxInputSelector**: Stream, DmNax, Hdmi1, Hdmi2, UsbC1, UsbC2, AnalogAudio, DanteAes67, DM, Arc, Bts, Noop

**NvxOutputSelector**: Stream, DmNax, Hdmi, AnalogAudio, DanteAes67, Arc, Bts, Noop

### Bridge Layer (SIMPL Integration)

`NvxDevice` implements `IBridgeAdvanced` and exposes state to SIMPL programs via `LinkToApi()`. The join map is defined in `NvxDeviceJoinMap.cs`.

Key join numbers:
- **Digital 1**: Device Online
- **Digital 11-12**: HDMI 1/2 Sync Detected
- **Analog/Serial 3**: Video Input Source
- **Analog/Serial 4**: Audio Input Source
- **Serial 21**: Stream URL
- **Serial 22**: Multicast Video Address
- **Serial 23-24**: NAX TX/RX Address
- **Analog 30+**: Network port (LLDP) information

### Messenger Layer (AppServer Integration)

Each device registers multiple `MessengerBase` subclasses for REST/WebSocket communication with modern UIs. Messengers are in the `McMessengers/` directory. They use debounce timers (100-200ms) to batch feedback updates and serialize state via `PostStatusMessage()`.

## Supported Device Types

The factory supports these `type` values (case-insensitive):

**Encoders/Transmitters**: DmNvx350, 350C, 351, 351C, 352, 352C, 363, 363C, 384, 384C, E10, E20, E202G, E30, E30C, E31, E31C, E760, E760C

**Decoders/Receivers**: DmNvx360, 360C, D30, D30C

**Testing**: mockNvxDevice

**XIO Directors**: xiodirector, xiodirector80, xiodirector160

## Configuration Model

`NvxDeviceProperties` has these fields:
- `DeviceId` (int) - virtual routing slot number, unique per TX or RX
- `Control` (ControlPropertiesConfig) - IPID and connection method
- `Mode` (string) - `"tx"` or `"rx"`
- `StreamUrl` (string) - initial stream URL for receivers
- `MulticastAddress` (string) - multicast address for transmitters
- `DmNaxTransmitAddress` (string) - DM-NAX transmit address
- `DmNaxReceiveAddress` (string) - DM-NAX receive address

## Dependencies

- **PepperDashEssentials** v2.36.2+ (core framework, includes Crestron SDK)
- **.NET Framework 4.7.2**
- **C# latest** with nullable reference types enabled

## Conventions

- Device keys are lowercase with hyphens (e.g., `nvx-tx-1`, `nvx-rx-1`)
- Feedback names match property names (PascalCase)
- All hardware events are subscribed in `Initialize()` and fire feedback updates
- The `SERIES4` compilation symbol is always defined
- No commented-out code; unused code is deleted
- Conventional commits: `feat:`, `fix:`, `chore:`
