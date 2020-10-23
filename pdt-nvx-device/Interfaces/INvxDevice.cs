using System;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Interfaces
{
    public interface INvxDevice : IRouting
    {
        RoutingOutputPort RoutingVideoOutput { get; }
        RoutingOutputPort RoutingAudioOutput { get; }

        Feedback VideoSourceFb { get; }
        Feedback AudioSourceFb { get; }
        Feedback HdmiInput1HdmiCapabilityFb { get; }
        Feedback DeviceNameFb { get; }
        Feedback CurrentlyRoutedVideoSourceFb { get; }
        Feedback CurrentlyRoutedAudioSourceFb { get; }
        Feedback OutputResolutionFb { get; }
        Feedback HdmiInput1SyncDetectedFb { get; }
        Feedback IsOnlineFb { get; }

        bool IsTransmitter { get; }
        int AudioInputSource { get; set; }
        int AudioSource { get; set; }
        int DeviceMode { get; }
        string DeviceName { get; }
        string ParentRouterKey { get; }
        string StreamUrl { get; }
        string MulticastAudioAddress { get; }
        string MulticastVideoAddress { get; }
        int HdmiInput1HdmiCapability { get; set; }
        int VideoInputSource { get; set; }
        int VideoSource { get; set; }
        int VirtualDevice { get; }
        string LocalUsbId { get; }
        string RemoteUsbId { get; set; }
        void Pair();
        void RemovePairing();
    }
}