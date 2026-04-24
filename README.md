# PepperDash NVX Plugin

The NVX plugin endeavors to provide device control and routing over Crestron NVX type devices without the need for an XIO director.

## Essentials Version

The NVX plugin current requires Essentials 2.7.4 or later.

> [!IMPORTANT]
> The name property in the Esssentials Device config is what the actual NVX device will be named.  This value must not contain any spaces or special characters.  

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
        "multicastAudioAddress": "239.0.0.3",
        "usb": {
            "mode": "local",
            "default": "nvx-decoder11",
            "followVideo": false
        }
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
        "deviceId": 1,
        "usb": {
            "mode": "remote",
            "default": "nvx-encoder5",
            "followVideo": false
        }
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

## Feature Requests

## NVX Router

When using the `nvxRouter` the following `nvxRoutingPort` types are available.

```
stream
hdmi1
hdmi2
analogAudio
primaryAudio
secondaryAudio
danteAudio
dmNaxAudio
automatic
noSwitch
```

### NVX Router Config Example

```json
{
    "key": "NvxRouter",
    "name": "NvxRouter",
    "type": "dynNvx",
    "group": "nvx",
    "properties": {
        "transmitters": {
            "1"  : { "deviceKey": "nvxTx1" , "videoName": "Input 1" , "nvxRoutingPort": "hdmi1" },
            "2"  : { "deviceKey": "nvxTx2" , "videoName": "Input 2" , "nvxRoutingPort": "hdmi1" },
            "3"  : { "deviceKey": "nvxTx3" , "videoName": "Input 3" , "nvxRoutingPort": "hdmi1" },
            "4"  : { "deviceKey": "nvxTx4" , "videoName": "Input 4" , "nvxRoutingPort": "hdmi1" }
        },
        "receivers": {
            "1"  : { "deviceKey": "nvxRx1" , "videoName": "Output 1"      },
            "2"  : { "deviceKey": "nvxRx2" , "videoName": "Output 2"      },
            "3"  : { "deviceKey": "nvxRx3" , "videoName": "Output 3"      },
            "4"  : { "deviceKey": "nvxRx4" , "videoName": "Output 4"      }
        },
        "audioTransmitters": {
            "1" : { "deviceKey": "nvxTx1" , "audioName": "Input 1" , "nvxRoutingPort": "hdmi1"       },
            "2" : { "deviceKey": "nvxTx2" , "audioName": "Input 2" , "nvxRoutingPort": "hdmi1"       },
            "3" : { "deviceKey": "nvxTx3" , "audioName": "Input 3" , "nvxRoutingPort": "hdmi1"       },
            "4" : { "deviceKey": "nvxTx4" , "audioName": "Input 4" , "nvxRoutingPort": "analogAudio" }
        },
        "audioReceivers": {
            "1"  : { "deviceKey": "nvxTx1" , "audioName": "Output 1"  },
            "2"  : { "deviceKey": "nvxTx2" , "audioName": "Output 2"  },
            "3"  : { "deviceKey": "nvxTx3" , "audioName": "Output 3"  },
            "4"  : { "deviceKey": "nvxTx4" , "audioName": "Output 4"  }
        }
    }
}
```

## USB Routing

- Local = `PC`
- Remote = `Keyboard/Mouse`

### NVX Plugin Device ID Offsets (when bridged)

- Encoder-to-Decoder: `0 + deviceId`
- Decoder-to-Decoder: `1000 + deviceId`

### NVX TX USB Configuration Object

```json
"usb": { "mode": "{local OR remote}", "followVideo": false, "isLayer3": false}
```

### NVX RX USB Configuration Object

```json
"usb": { "mode": "{local OR remote}", "followVideo": false, "isLayer3": false }
```
