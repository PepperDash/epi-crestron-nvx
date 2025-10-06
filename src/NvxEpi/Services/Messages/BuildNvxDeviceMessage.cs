using System;
using Crestron.SimplSharpPro;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Queues;

namespace NvxEpi.Services.Messages;

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
            this.LogError("Device registration failed! '{0}'", _device.RegistrationFailureReason.ToString());
            throw new Exception(_device.RegistrationFailureReason.ToString());
        }
    }

    public string Key { get; private set; }
}