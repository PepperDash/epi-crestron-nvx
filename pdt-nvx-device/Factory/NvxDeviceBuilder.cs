using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Endpoints;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Essentials.Core;

namespace NvxEpi.Factory
{
    public class NvxDeviceBuilder
    {
        private readonly string _deviceName;
        private readonly NvxDevicePropertiesConfig _config;

        public NvxDeviceBuilder(string deviceName, NvxDevicePropertiesConfig config)
        {
            _deviceName = deviceName;
            _config = config;
        }

        public DmNvxBaseClass GetNvxDevice()
        {
            try
            {
                var nvxDeviceType = typeof(DmNvxBaseClass)
                    .GetCType()
                    .Assembly
                    .GetTypes()
                    .FirstOrDefault(x => x.Name.Equals(_config.Model, StringComparison.OrdinalIgnoreCase));

                if (nvxDeviceType == null) throw new NullReferenceException();
                if (_config.Control.IpId == null) throw new Exception("The IPID for this device must be defined");

                var newDevice = nvxDeviceType
                    .GetConstructor(new CType[] { typeof(ushort).GetCType(), typeof(CrestronControlSystem) })
                    .Invoke(new object[] { _config.Control.IpIdInt, Global.ControlSystem });

                var nvxDevice = newDevice as DmNvxBaseClass;
                if (nvxDevice == null) throw new NullReferenceException("Could not find the base nvx type");

                if (_config.DeviceName != null) nvxDevice.Control.Name.StringValue = _config.DeviceName.Replace(" ", string.Empty);
                else nvxDevice.Control.Name.StringValue = _deviceName.Replace(" ", string.Empty);

                return nvxDevice;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}