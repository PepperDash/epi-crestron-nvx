using System;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using EssentialsExtensions;
using EssentialsExtensions.Attributes;

namespace NvxEpi.Interfaces
{
    public interface INvxDevice : IRouting
    {
        RoutingOutputPort RoutingVideoOutput { get; }
        RoutingOutputPort RoutingAudioOutput { get; }

        bool IsTransmitter { get; }
        bool IsReceiver { get; }
        int AudioInputSource { get; }
        int AudioSource { get; }
        int DeviceMode { get; }
        string DeviceName { get; }
        string StreamUrl { get; }
        string MulticastAudioAddress { get; }
        string MulticastVideoAddress { get; }
        int VideoInputSource { get; set; }
        int VideoSource { get; set; }
        int VirtualDevice { get; }
        string LocalUsbId { get; }
        string RemoteUsbId { get; set; }
        void Pair();
        void RemovePairing();
    }
}