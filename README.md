# PepperDash NVX Plugin

The NVX plugin endeavors to provide device control and routing over Crestron NVX type devices without the need for an XIO director.

## Essentials Version

The NVX plugin current requires Essentials 2.7.4 or later.

### __IMPORTANT:__

The name property in the Esssentials Device config is what the actual NVX device will be named.  This value must not contain any spaces or special characters.  

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
<!-- START Minimum Essentials Framework Versions -->

<!-- END Minimum Essentials Framework Versions -->
<!-- START Config Example -->
### Config Example

```json
{
    "key": "GeneratedKey",
    "uid": 1,
    "name": "GeneratedName",
    "type": "amplifier",
    "group": "Group",
    "properties": {
        "transmitters": {
            "SampleString": {
                "DeviceKey": "SampleString",
                "VideoName": "SampleString",
                "NvxRoutingPort": "SampleString"
            }
        },
        "receivers": {
            "SampleString": {
                "DeviceKey": "SampleString",
                "VideoName": "SampleString",
                "NvxRoutingPort": "SampleString"
            }
        },
        "audioTransmitters": {
            "SampleString": {
                "DeviceKey": "SampleString",
                "AudioName": "SampleString",
                "NvxRoutingPort": "SampleString"
            }
        },
        "audioReceivers": {
            "SampleString": {
                "DeviceKey": "SampleString",
                "AudioName": "SampleString",
                "NvxRoutingPort": "SampleString"
            }
        }
    }
}
```
<!-- END Config Example -->
<!-- START Supported Types -->
### Supported Types

- amplifier
<!-- END Supported Types -->
<!-- START Join Maps -->

<!-- END Join Maps -->
<!-- START Interfaces Implemented -->
### Interfaces Implemented

- IComPorts
- IIROutputPorts
- IUsbStreamWithHardware
- IHdmiInput
- IVideowallMode
- IRoutingWithFeedback
- ICec
- INvx35XDeviceWithHardware
- IStream
- ISecondaryAudioStream
- IRoutingNumeric
- IBridgeAdvanced
- IHasFeedback
- IUsbcInput
- IMultiview
- IBasicVolumeWithFeedback
- INvxDirector
- IOnline
- ICommunicationMonitor
- INvxD3XDeviceWithHardware
- IHdmiOutput
- ICurrentVideoInput
- ICurrentAudioInput
- ICurrentStream
- ICurrentSecondaryAudioStream
- ICurrentNaxInput
- IDeviceInfoProvider
- INvxNetworkPortInformation
- INvxE3XDeviceWithHardware
- IQueueMessage
- IKeyed
- IHandleInputSwitch
- IRoutingSink
- INvxApplicationBuilder
- ISecondaryAudioStreamWithHardware
- IStreamWithHardware
- ICurrentDanteInput
- IRoutingOutputSlot
- IRoutingInputSlot
- IMatrixRouting
<!-- END Interfaces Implemented -->
<!-- START Base Classes -->
### Base Classes

- NvxBaseDevice
- ReconfigurableDevice
- EssentialsDevice
- EssentialsBridgeableDevice
- JoinMapBaseAdvanced
- MessengerBase
- SecondaryAudioStream
- VideoStream
- UsbcInputBase
- HdmiInputBase
- HdmiOutput
- StatusMonitorBase
- BaseStreamingDeviceProperties
- Enumeration<DeviceModeEnum>
- Enumeration<DeviceInputEnum>
- Enumeration<StreamingStatusEnum>
- Enumeration<VideoOutputEnum>
- Enumeration<AudioOutputEnum>
- Enumeration<NaxInputEnum>
- Enumeration<AudioInputEnum>
- Enumeration<VideoInputEnum>
- Enumeration<HdcpCapabilityEnum>
- NvxBaseDeviceFactory<Nvx38X>
- NvxBaseDeviceFactory<NvxMockDevice>
- NvxBaseDeviceFactory<NvxD3X>
- NvxBaseDeviceFactory<NvxE3X>
- NvxBaseDeviceFactory<Nvx36X>
- NvxBaseDeviceFactory<Nvx35X>
<!-- END Base Classes -->
<!-- START Public Methods -->
### Public Methods

- public void ClearCurrentUsbRoute()
- public void MakeUsbRoute(IUsbStreamWithHardware hardware)
- public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
- public void SetSyncState(bool state)
- public void SetIsOnline(bool state)
- public void LinkToApi(
        BasicTriList trilist,
        uint joinStart,
        string joinMapKey,
        EiscApiAdvanced bridge
    )
- public void ClearCurrentUsbRoute()
- public void MakeUsbRoute(IUsbStreamWithHardware hardware)
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void VolumeUp(bool pressRelease)
- public void VolumeDown(bool pressRelease)
- public void MuteToggle()
- public void SetVolume(ushort level)
- public void MuteOn()
- public void MuteOff()
- public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
- public void ClearCurrentUsbRoute()
- public void MakeUsbRoute(IUsbStreamWithHardware hardware)
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void VolumeUp(bool pressRelease)
- public void VolumeDown(bool pressRelease)
- public void MuteToggle()
- public void SetVolume(ushort level)
- public void MuteOn()
- public void MuteOff()
- public void UpdateDeviceInfo()
- public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
- public void Dispatch()
- public void HandleSwitch(object input, eRoutingSignalType type)
- public void HandleSwitch(object input, eRoutingSignalType type)
- public void HandleSwitch(object input, eRoutingSignalType type)
- public void HandleSwitch(object input, eRoutingSignalType type)
- public void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
- public void SetAudioFollowsVideoTrue()
- public void SetAudioFollowsVideoFalse()
- public void SetHdcpState(ushort state)
- public EssentialsDevice Build()
- public void UpdateCurrentRoute()
- public void MakeUsbRoute(IUsbStreamWithHardware hardware)
- public void ClearCurrentUsbRoute()
- public void ClearRemoteUsbRoute()
- public void VolumeUp(bool pressRelease)
- public void VolumeDown(bool pressRelease)
- public void MuteToggle()
- public void SetVolume(ushort level)
- public void MuteOn()
- public void MuteOff()
- public void VolumeUp(bool pressRelease)
- public void VolumeDown(bool pressRelease)
- public void MuteToggle()
- public void SetVolume(ushort level)
- public void MuteOn()
- public void MuteOff()
- public void VolumeUp(bool pressRelease)
- public void VolumeDown(bool pressRelease)
- public void MuteToggle()
- public void SetVolume(ushort level)
- public void MuteOn()
- public void MuteOff()
- public void CheckIfDeviceIsOnlineAndUpdate()
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void TestUsbRoute(string inputPortKey, string outputPortKey)
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void ExecuteSwitch(
        object inputSelector,
        object outputSelector,
        eRoutingSignalType signalType
    )
- public void ExecuteNumericSwitch(ushort input, ushort output, eRoutingSignalType type)
- public void Route(string inputSlotKey, string outputSlotKey, eRoutingSignalType type)
- public int CompareTo(Enumeration<TEnum> other)
- public int CompareTo(object other)
<!-- END Public Methods -->
<!-- START Bool Feedbacks -->
### Bool Feedbacks

- DisabledByHdcp
- IsOnline
- IsStreamingVideo
- IsStreamingSecondaryAudio
- SyncDetected
- DisabledByHdcp
- MultiviewEnabled
- MuteFeedback
- IsOnline
- DisabledByHdcp
- DisabledByHdcp
- MuteFeedback
- IsStreamingVideo
- IsStreamingSecondaryAudio
- IsOnline
- DisabledByHdcp
- IsOnline
- HdmiSyncDetected
- IsOnline
- IsOnline
- IsStreamingSecondaryAudio
- IsOnline
- IsStreamingVideo
- IsOnline
- IsOnline
- MuteFeedback
- MuteFeedback
- MuteFeedback
- IsOnline
- DisabledByHdcp
- IsOnline
- IsOnline
- IsOnline
- IsOnline
- IsOnline
- IsOnline
- IsOnline
- IsOnline
<!-- END Bool Feedbacks -->
<!-- START Int Feedbacks -->
### Int Feedbacks

- HorizontalResolution
- VideoAspectRatioMode
- VideowallMode
- DeviceMode
- HorizontalResolution
- VideoAspectRatioMode
- VideowallMode
- MultiviewLayout
- VolumeLevelFeedback
- HorizontalResolution
- HorizontalResolution
- VideoAspectRatioMode
- VideowallMode
- VolumeLevelFeedback
- CurrentAudioInputValue
- CurrentNaxInputValue
- CurrentVideoInputValue
- DeviceMode
- CurrentStreamId
- CurrentSecondaryAudioStreamId
- CurrentVideoRouteId
- HorizontalResolution
- AspectRatioMode
- HdcpState
- HdcpCapability
- CurrentAudioRouteId
- DeviceMode
- CurrentSecondaryAudioStreamId
- CurrentStreamId
- DeviceMode
- DeviceMode
- DeviceMode
- VolumeLevelFeedback
- VolumeLevelFeedback
- VolumeLevelFeedback
- DeviceMode
- VideowallMode
- VideoAspectRatioMode
- HorizontalResolution
- DeviceMode
- DeviceMode
- CurrentDanteInputValue
- CurrentAudioInputValue
- DeviceMode
- CurrentNaxInputValue
- DeviceMode
- CurrentVideoInputValue
- DeviceMode
<!-- END Int Feedbacks -->
<!-- START String Feedbacks -->
### String Feedbacks

- EdidManufacturer
- OutputResolution
- UsbLocalId
- StreamUrl
- SecondaryAudioAddress
- TxAudioAddress
- RxAudioAddress
- VideoStreamStatus
- SecondaryAudioStreamStatus
- MulticastAddress
- EdidManufacturer
- OutputResolution
- WindowAStreamUrl
- WindowBStreamUrl
- WindowCStreamUrl
- WindowDStreamUrl
- WindowEStreamUrl
- WindowFStreamUrl
- UsbLocalId
- EdidManufacturer
- OutputResolution
- EdidManufacturer
- OutputResolution
- UsbLocalId
- CurrentAudioInput
- CurrentNaxInput
- CurrentVideoInput
- StreamUrl
- VideoStreamStatus
- CurrentStreamName
- SecondaryAudioAddress
- TxAudioAddress
- RxAudioAddress
- SecondaryAudioStreamStatus
- CurrentSecondaryAudioStreamName
- MulticastAddress
- NameFeedback
- VideoName
- CurrentVideoRouteName
- EdidManufacturer
- AudioName
- InputResolution
- NameFeedback
- VideoName
- AudioName
- CurrentAudioRouteName
- SecondaryAudioStreamStatus
- SecondaryAudioAddress
- TxAudioAddress
- RxAudioAddress
- CurrentSecondaryAudioStreamName
- CurrentStreamName
- VideoStreamStatus
- StreamUrl
- MulticastAddress
- UsbLocalId
- EdidManufacturer
- OutputResolution
- CurrentDanteInput
- CurrentAudioInput
- CurrentNaxInput
- CurrentVideoInput
<!-- END String Feedbacks -->
