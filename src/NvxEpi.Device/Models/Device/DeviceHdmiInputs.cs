using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using NvxEpi.Device.Abstractions;

namespace NvxEpi.Device.Models.Device
{
    public class DeviceHdmiInputs : IHasHdmiInputs
    {
        private readonly INvxDevice _device;
        private readonly Dictionary<uint, DeviceHdmiInput> _inputs = new Dictionary<uint, DeviceHdmiInput>();
 
        public DeviceHdmiInputs(INvxDevice device)
        {
            _device = device;
            Initialize();
        }

        private void Initialize()
        {
            if (_device.Hardware.HdmiIn == null)
                throw new NotSupportedException("Hdmi In");

            for (uint x = 1; x <= _device.Hardware.HdmiIn.Count; x++)
            {
                _inputs.Add(x, new DeviceHdmiInput((int)x, _device));
            }
        }

        public ReadOnlyDictionary<uint, DeviceHdmiInput> HdmiInputs 
        {
            get { return new ReadOnlyDictionary<uint, DeviceHdmiInput>(_inputs); }
        }
    }
}