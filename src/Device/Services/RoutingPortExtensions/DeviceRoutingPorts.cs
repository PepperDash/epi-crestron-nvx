using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Models;

namespace NvxEpi.Device.Services.RoutingPortExtensions
{
    public static class DeviceRoutingPorts
    {
        public static NvxDevice BuildRoutingPorts(this NvxDevice device)
        {
            if (device.Hardware is DmNvx35x)
                device.BuildFor35X();

            else if (device.Hardware is DmNvxE3x)
                device.BuildForE3X();

            else if (device.Hardware is DmNvxD3x)
                device.BuildForD3X();

            else
                throw new ArgumentException("device");

            return device;
        }

        public static void BuildForE3X(this NvxDevice device)
        {
            if (!(device.Hardware is DmNvxE3x))
                throw new ArgumentException("device");

            VideoInputEnum.Hdmi1.AddRoutingPortToDevice(device);
            VideoOutputEnum.Stream.AddRoutingPortToDevice(device);
            AudioInputEnum.AnalogAudio.AddRoutingPortToDevice(device);
        }

        public static void BuildForD3X(this NvxDevice device)
        {
            if (!(device.Hardware is DmNvxD3x))
                throw new ArgumentException("device");

            VideoInputEnum.Stream.AddRoutingPortToDevice(device);
            VideoOutputEnum.Hdmi.AddRoutingPortToDevice(device);
            AudioOutputEnum.Analog.AddRoutingPortToDevice(device);
        }

        public static void BuildFor35X(this NvxDevice device)
        {
            if (!(device.Hardware is DmNvx35x))
                throw new ArgumentException("device");

            VideoInputEnum.Hdmi1.AddRoutingPortToDevice(device);
            VideoInputEnum.Hdmi2.AddRoutingPortToDevice(device);
            VideoOutputEnum.Hdmi.AddRoutingPortToDevice(device);

            if (device.IsTransmitter)
                device.BuildFor35XTx();
            else
                device.BuildFor35XRx();
        }

        private static void BuildFor35XRx(this NvxDevice device)
        {
            VideoInputEnum.Stream.AddRoutingPortToDevice(device);
            AudioOutputEnum.Analog.AddRoutingPortToDevice(device);
        }

        private static void BuildFor35XTx(this NvxDevice device)
        {
            VideoOutputEnum.Stream.AddRoutingPortToDevice(device);
            AudioInputEnum.AnalogAudio.AddRoutingPortToDevice(device);
        }
    }
}