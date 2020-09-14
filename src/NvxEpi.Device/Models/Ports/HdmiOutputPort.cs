using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Abstractions;
using NvxEpi.Device.Enums;
using NvxEpi.Device.Services.Utilities;
using PepperDash.Essentials.Core;

namespace NvxEpi.Device.Models.Ports
{
    public class HdmiOutputRoutingCommand : IRoutingCommand
    {
        private readonly INvxDevice _device;

        public HdmiOutputRoutingCommand(INvxDevice device)
        {
            _device = device;
        }

        public string Key { get { return _device.Key; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <exception cref="input">ArgumentNullException</exception>
        public void HandleRoute(RoutingInputPortEnum input, eRoutingSignalType type)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Equals(RoutingInputPortEnum.Stream))
                HandleRouteStream(type);

            if (input.Equals(RoutingInputPortEnum.SecondaryAudio))
                HandleRouteSecondaryAudio(type);

        }

        private void HandleRouteStream(eRoutingSignalType type)
        {
            if (type.Has(eRoutingSignalType.Video))
                _device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;

            if (type.Has(eRoutingSignalType.Audio))
                _device.Hardware.Control.AudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
        }

        private void HandleRouteSecondaryAudio(eRoutingSignalType type)
        {
            if (type.Has(eRoutingSignalType.Video))
                throw new ArgumentException("type cannot be video only");

            if (type.Has(eRoutingSignalType.Audio))
            {
                
            }

        }
    }
}