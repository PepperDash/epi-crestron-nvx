using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Devices;
using Crestron.SimplSharpPro;
using PepperDash.Essentials.Core.Queues;

namespace NvxEpi.Services.Messages
{
    public class BuildNvxDeviceMessage : IQueueMessage, IKeyed
    {
        private readonly GenericBase _device;

        public BuildNvxDeviceMessage(string key, GenericBase @base)
        {
            Key = key;
            _device = @base;
        }

        public void Dispatch()
        {
            if (_device.Registered)
                return;

            var result = _device.RegisterWithLogging(Key);

            if (result != eDeviceRegistrationUnRegistrationResponse.Success)
            {
                Debug.Console(1, this, Debug.ErrorLogLevel.Warning, "Device registration failed! '{0}'", _device.RegistrationFailureReason.ToString());
                throw new Exception(_device.RegistrationFailureReason.ToString());
            }
        }
    
        public string Key { get; private set; }
    }
}