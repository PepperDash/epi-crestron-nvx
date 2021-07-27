# PepperDash NVX Plugin

> The NVX plugin endeavors to provide device control and routing over Crestron NVX type devices without the need for an XIO director.

## Installation

Navigate to the BUILDS folder in the repository.  Place the .cplz file into the Plugins folder for Essentials and reset the application.

### __IMPORTANT:__

The name property in the Esssentials Device config is what the actual NVX device will be named.  This value must not contain any spaces or special characters.  

<strong>This plugin does not currently support E30/D30 encoders and decoders.  They will throw null reference exeptions.  This is in development.</strong>

## Join Map

See details section below for detailed description of device properties.

### Joins

| Join | Digital                | Analog                   | Serial                   |
| ---- | ---------------------- | ------------------------ | ------------------------ |
| 1    | Device Online          | Video Route              | Video Route              |
| 2    | Stream Started         | Audio Route              | Audio Route              |
| 3    | HDMI01 Sync Detected   | Video Input              | Video Input              |
| 4    | HDMI02 Sync Detected   | Audio Input              | Audio Input              |
| 5    | -                      | USB Route                | USB Route                |
| 6    | Supports HDMI01        | HDMI01 HDCP Capability   | HDMI01 HDCP Capability   |
| 7    | Supports HDMI02        | HDMI02 HDCP Capability   | HDMI02 HDCP Capability   |
| 8    | Output Disabled by Hdcp| Hdmi Output Res.         | Hdmi Output Res.         |
| 9    | Supports Videowall     | Videowall Mode           | -                        |
| 10   | -                      | Video Aspect Ratio Mode  | Dante Input              |
| 11   | -                      | Nax Input                | Device Name              |
| 12   | Supports Nax           | Nax Route                | Nax Route                |
| 13   | -                      | Nax Input                | Nax Input                |
| 14   | -                      | -                        | Stream Url               |
| 15   | -                      | -                        | Multicast Video Address  |
| 16   | -                      | -                        | Secondary Audio Address  |
| 17   | -                      | -                        | NAX Tx Address           |
| 18   | -                      | -                        | NAX Rx Address           |

## Join Details

1. Video Source = In RX mode; will route the video stream of the device with the Virtual ID of the value selected to the receiver.  No action in TX mode.
2. Audio Source = In RX mode; will route the audio stream of the device with the Virtual ID of the value selected to the receiver.  No action in TX mode.
3. Video Input Source = Selects the video source of the TX/RX.  DISABLE = 0, Hdmi1 = 1, Hdmi2 = 2, Stream = 3.  This is set automatically in config; do not recommend changing.
4. Audio Input Source = Selected the audio source of the TX/RX.  AUTOMATIC = 0, NoAudioSelected = 0, Input1 = 1, Input2 = 2, Analog Audio = 3, PrimaryStreamAudio = 4, Secondary Stream Audio = 5, Dante = 6(Not yet supported).  This is set automatically in config; do not recommend changing.
5. Device Mode = Reports the device mode.  RX = 0, TX = 1
6. HDCP Capability = Reports and sets HDCP cabaility on selected input.  HDCPSupportOff = 0, HDCPSupportAuto = 1, HDCP1xSupport = 2, HDCP2xSupport = 3
7. HDCP Supported Level = UNUSED for NVX devices
8. HDMI Output Horizontal Resolution = Custom Wharton usage; reports the horizontal resolution of the connected display on the HDMI output.

## Config Example

```JSON
{
    "key": "Tx-1",
    "uid": 1,
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
        "multicastVideoAddress": "239.0.0.2",
        "multicastAudioAddress": "239.0.0.3"
    }
},
{
    "key": "Rx-1",
    "uid": 74,
    "name": "PC-Cam",
    "type": "DmNvx363",
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

### Config Details

__Properties:__

1. IPID = sets the IPID of the device.
2. Model = Case-insentive model number of the actual NVX device.
3. Mode = VALID VALUES: { "tx", "rx" }; sets the device as a transmitter or receiver.  This must be defined or else the application
will throw an exception.  Many default values are set based on this property.  Adjusting this value via the web interface is _not_ supported.
4. Device Id: A unique number that can be thought of as an "input" or "output" number when performing routing.  For example, if you required to route
TX-1 to RX-1, you would send a value of 1 to the Video Source input of RX-1.
5. Multicast Video Address = Sets the local multicast Video address that a transmitter will attempt to stream to.  This address __must__ have an even number as the last Octet and is recommended to fall within a locally scoped mulitcast address range. Recommended reading : [http://www.tcpipguide.com/free/t_IPMulticastAddressing.htm]
6. Multicast Audio Address = Sets the local multicast Audio address that a transmitter will attempt to stream the secondary audio to.  In most cases set this to be +1 of the Multicast Video address.

## Planned Updates

1. Add a name property in the config file to set the device to.
2. Add a "Friendly Name" property in the config file to show for client facing UI purposes.
3. Add XIO director support.
4. Add a "Virtual Routing" plugin to mimic DynDM API to make easy drop in changes.
5. Add and test Dante Audio routing.

## Feature Requests
