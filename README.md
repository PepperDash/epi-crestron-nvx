# PepperDash NVX Plugin

The NVX plugin endeavors to provide device control and routing over Crestron NVX type devices without the need for an XIO director.

## Essentials Version

The NVX plugin current requires Essentials 2.17.0 or later.

### __IMPORTANT:__

The name property in the Esssentials Device config is what the actual NVX device will be named.  This value must not contain any spaces or special characters.  

## Join Map

See details section below for detailed description of device properties.

### Joins

| Join | Digital                 | Analog                   | Serial                   |
| ---- | ----------------------- | ------------------------ | ------------------------ |
| 1    | Device Online           | Video Route              | Video Route              |
| 2    | Stream Started          | Audio Route              | Audio Route              |
| 3    | HDMI01 Sync Detected    | Video Input              | Video Input              |
| 4    | HDMI02 Sync Detected    | Audio Input              | Audio Input              |
| 5    | -                       | USB Route                | USB Route                |
| 6    | Supports HDMI01         | HDMI01 HDCP Capability   | -                        |
| 7    | Supports HDMI02         | HDMI02 HDCP Capability   | -                        |
| 8    | Output Disabled by HDCP | HDMI Output Resolution   | Hdmi Output Resolution   |
| 9    | Supports Videowall      | Videowall Mode           | -                        |
| 10   | -                       | Dante Input              | Dante Input              |
| 11   | -                       | Video Aspect Ratio Mode  | Device Name              |
| 12   | Supports Nax            | Nax Route                | Nax Route                |
| 13   | -                       | Nax Input                | Nax Input                |
| 14   | -                       | -                        | Stream Url               |
| 15   | -                       | -                        | Multicast Video Address  |
| 16   | -                       | -                        | Secondary Audio Address  |
| 17   | -                       | -                        | NAX Tx Address           |
| 18   | -                       | -                        | NAX Rx Address           |

MultiviewJoinMap - Only visible if configuration objects exist

| Join | Digital                 | Analog                   | Serial                   |
| ---- | ----------------------- | ------------------------ | ------------------------ |
| 21   | Enter                   | Layout                   | Window 1 Stream Url      |
| 22   | Enable                  | Audio Route              | Window 2 Stream Url      |
| 23   | Disable                 | -                        | Window 3 Stream Url      |
| 24   | -                       | -                        | Window 4 Stream Url      |
| 25   | -                       | -                        | Window 5 Stream Url      |
| 26   | -                       | -                        | Window 6 Stream Url      |
| 27   | -                       | -                        | -                        |
| 28   | -                       | -                        | -                        |
| 29   | -                       | -                        | -                        |
| 30   | -                       | -                        | -                        |
| 31   | -                       | -                        | Window 1 Label           |
| 32   | -                       | -                        | Window 2 Label           |
| 33   | -                       | -                        | Window 3 Label           |
| 34   | -                       | -                        | Window 4 Label           |
| 35   | -                       | -                        | Window 5 Label           |
| 36   | -                       | -                        | Window 6 Label           |

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
    "name": "Display-1",
    "type": "DmNvx384",
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
        },
        "screens": {
            "1": {
                "enabled": true,
                "name": "Main Screen",
                "screenIndex": 1,
                "layouts": {
                "1": {
                    "layoutName": "Full Screen",
                    "layoutIndex": 0,
                    "layoutType": "fullScreen",
                    "windows": {
                    "1": {
                        "label": "Full Screen",
                        "input": "input1"
                    }
                    }
                },
                "2": {
                    "layoutName": "Side By Side",
                    "layoutIndex": 201,
                    "layoutType": "sideBySide",
                    "windows": {
                    "1": {
                        "label": "Left Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Right Window",
                        "input": "input2"
                    }
                    }
                },
                "3": {
                    "layoutName": "PIP Small Top Left",
                    "layoutIndex": 202,
                    "layoutType": "pipSmallTopLeft",
                    "windows": {
                    "1": {
                        "label": "Main Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "PIP Window",
                        "input": "input2"
                    }
                    }
                },
                "4": {
                    "layoutName": "PIP Small Top Right",
                    "layoutIndex": 203,
                    "layoutType": "pipSmallTopRight",
                    "windows": {
                    "1": {
                        "label": "Main Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "PIP Window",
                        "input": "input2"
                    }
                    }
                },
                "5": {
                    "layoutName": "PIP Small Bottom Left",
                    "layoutIndex": 204,
                    "layoutType": "pipSmallBottomLeft",
                    "windows": {
                    "1": {
                        "label": "Main Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "PIP Window",
                        "input": "input2"
                    }
                    }
                },
                "6": {
                    "layoutName": "PIP Small Bottom Right",
                    "layoutIndex": 205,
                    "layoutType": "pipSmallBottomRight",
                    "windows": {
                    "1": {
                        "label": "Main Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "PIP Window",
                        "input": "input2"
                    }
                    }
                },
                "7": {
                    "layoutName": "1 Top, 2 Bottom",
                    "layoutIndex": 301,
                    "layoutType": "oneTopTwoBottom",
                    "windows": {
                    "1": {
                        "label": "Top Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Bottom Left",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Bottom Right",
                        "input": "input3"
                    }
                    }
                },
                "8": {
                    "layoutName": "2 Top, 1 Bottom",
                    "layoutIndex": 302,
                    "layoutType": "twoTopOneBottom",
                    "windows": {
                    "1": {
                        "label": "Top Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Top Right",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Bottom Window",
                        "input": "input3"
                    }
                    }
                },
                "9": {
                    "layoutName": "1 Left, 2 Right",
                    "layoutIndex": 303,
                    "layoutType": "oneLeftTwoRight",
                    "windows": {
                    "1": {
                        "label": "Left Window",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Right Top",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Right Bottom",
                        "input": "input3"
                    }
                    }
                },
                "10": {
                    "layoutName": "2 Top, 2 Bottom",
                    "layoutIndex": 401,
                    "layoutType": "twoTopTwoBottom",
                    "windows": {
                    "1": {
                        "label": "Top Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Top Right",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Bottom Left",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Bottom Right",
                        "input": "input4"
                    }
                    }
                },
                "11": {
                    "layoutName": "1 Left, 3 Right",
                    "layoutIndex": 402,
                    "layoutType": "oneLeftThreeRight",
                    "windows": {
                    "1": {
                        "label": "Left Large",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Right Top",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Right Middle",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Right Bottom",
                        "input": "input4"
                    }
                    }
                },
                "12": {
                    "layoutName": "1 Large Left, 4 Right",
                    "layoutIndex": 501,
                    "layoutType": "oneLargeLeftFourRight",
                    "windows": {
                    "1": {
                        "label": "Large Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Small Right 1",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Small Right 2",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Small Right 3",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Small Right 4",
                        "input": "input5"
                    }
                    }
                },
                "13": {
                    "layoutName": "4 Left, 1 Large Right",
                    "layoutIndex": 502,
                    "layoutType": "fourLeftOneLargeRight",
                    "windows": {
                    "1": {
                        "label": "Small Left 1",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Small Left 2",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Small Left 3",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Small Left 4",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Large Right",
                        "input": "input5"
                    }
                    }
                },
                "14": {
                    "layoutName": "2 Left, 1 Large Center, 2 Right",
                    "layoutIndex": 503,
                    "layoutType": "twoLeftOneLargeCenterTwoRight",
                    "windows": {
                    "1": {
                        "label": "Left Top",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Left Bottom",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Large Center",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Right Top",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Right Bottom",
                        "input": "input5"
                    }
                    }
                },
                "15": {
                    "layoutName": "3 Top, 3 Bottom",
                    "layoutIndex": 601,
                    "layoutType": "threeTopThreeBottom",
                    "windows": {
                    "1": {
                        "label": "Top Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Top Center",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Top Right",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Bottom Left",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Bottom Center",
                        "input": "input5"
                    },
                    "6": {
                        "label": "Bottom Right",
                        "input": "input6"
                    }
                    }
                },
                "16": {
                    "layoutName": "1 Large Left, 5 Stacked",
                    "layoutIndex": 602,
                    "layoutType": "oneLargeLeftFiveStacked",
                    "windows": {
                    "1": {
                        "label": "Large Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Stack 1",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Stack 2",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Stack 3",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Stack 4",
                        "input": "input5"
                    },
                    "6": {
                        "label": "Stack 5",
                        "input": "input6"
                    }
                    }
                },
                "17": {
                    "layoutName": "5 Around, 1 Large Bottom Left",
                    "layoutIndex": 603,
                    "layoutType": "fiveAroundOneLargeBottomLeft",
                    "windows": {
                    "1": {
                        "label": "Large Bottom Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Small Top Left",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Small Top Center",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Small Top Right",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Small Bottom Center",
                        "input": "input5"
                    },
                    "6": {
                        "label": "Small Bottom Right",
                        "input": "input6"
                    }
                    }
                },
                "18": {
                    "layoutName": "5 Around, 1 Large Top Left",
                    "layoutIndex": 604,
                    "layoutType": "fiveAroundOneLargeTopLeft",
                    "windows": {
                    "1": {
                        "label": "Large Top Left",
                        "input": "input1"
                    },
                    "2": {
                        "label": "Small Top Center",
                        "input": "input2"
                    },
                    "3": {
                        "label": "Small Top Right",
                        "input": "input3"
                    },
                    "4": {
                        "label": "Small Bottom Left",
                        "input": "input4"
                    },
                    "5": {
                        "label": "Small Bottom Center",
                        "input": "input5"
                    },
                    "6": {
                        "label": "Small Bottom Right",
                        "input": "input6"
                    }
                    }
                }
                }
            }
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

### Multiview Details

#### Key Features:
- 18 different multiview layouts ranging from simple full screen to complex 6-window arrangements
- Layout categories by window count:
- - 1 window: Full Screen (layout 0)
- - 2 windows: Side by side, PIP variations (layouts 201-205)
- - 3 windows: Various arrangements (layouts 301-303)
- - 4 windows: Quad layouts (layouts 401-402)
- - 5 windows: Asymmetric layouts (layouts 501-503)
- - 6 windows: Grid and mixed layouts (layouts 601-604)

#### Layout Index Mapping:
The layout indexes correspond to what the hardware expects:

- 0: Full screen mode
- 200s: 2-window layouts
- 300s: 3-window layouts
- 400s: 4-window layouts
- 500s: 5-window layouts
- 600s: 6-window layouts

