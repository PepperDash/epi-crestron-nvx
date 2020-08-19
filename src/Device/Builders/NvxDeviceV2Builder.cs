using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp.Reflection;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Device.Models;
using NvxEpi.Device.Services.Config;
using NvxEpi.Device.Services.DeviceExtensions;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Device.Builders
{
    public abstract class NvxDeviceV2Builder : INvxDeviceBuilder
    {
        protected readonly NvxDeviceProperties _props;

        protected NvxDeviceV2Builder(DeviceConfig config)
        {
            Config = config;
            _props = NvxDeviceProperties.FromDeviceConfig(Config);
         
            Feedbacks = new Dictionary<NvxDevice.DeviceFeedbacks, Feedback>();
            BoolActions = new Dictionary<NvxDevice.BoolActions, Action<bool>>();
            IntActions = new Dictionary<NvxDevice.IntActions, Action<ushort>>();
            StringActions = new Dictionary<NvxDevice.StringActions, Action<string>>();

            Initialize();
        }

        private void Initialize()
        {
            Device = BuildDevice(Config.Type, _props);
            BuildFeedbacks();
            BuildActions();
        }

        public DmNvxBaseClass Device { get; private set; }
        public DeviceConfig Config { get; private set; }

        public bool IsTransmitter
        {
            get { return _props.Mode.Equals("tx", StringComparison.OrdinalIgnoreCase); }
        }

        public Dictionary<NvxDevice.DeviceFeedbacks, Feedback> Feedbacks { get; private set; }
        public Dictionary<NvxDevice.BoolActions, Action<bool>> BoolActions { get; private set; }
        public Dictionary<NvxDevice.IntActions, Action<ushort>> IntActions { get; private set; }
        public Dictionary<NvxDevice.StringActions, Action<string>> StringActions { get; private set; }

        public string Key
        {
            get { return Config.Key; }
        }

        public string Name
        {
            get { return Config.Name; }
        }

        public virtual NvxDevice Build()
        {
            if (DeviceManager.GetDeviceForKey(NvxRouter.Instance.Key) == null)
                DeviceManager.AddDevice(NvxRouter.Instance);

            if (Device is DmNvxE3x && !IsTransmitter)
                throw new NotSupportedException("cannot be a receiver");

            if (Device is DmNvxD3x && IsTransmitter)
                throw new NotSupportedException("cannot be a transmitter");

            var device = new NvxDevice(this)
                .RegisterForDeviceFeedback(Device);

            SetDeviceDefaults();
            BuildRoutingPorts(device);

            return device;
        }

        public abstract void SetDeviceDefaults();

        private static DmNvxBaseClass BuildDevice(string type, NvxDeviceProperties props)
        {
            var nvxDeviceType = typeof (DmNvxBaseClass)
                .GetCType()
                .Assembly
                .GetTypes()
                .FirstOrDefault(x => x.Name.Equals(type, StringComparison.OrdinalIgnoreCase));

            if (nvxDeviceType == null)
                throw new NullReferenceException("The type specified in the config file wasn't found");

            if (props.Control.IpId == null)
                throw new Exception("The IPID for this device must be defined");

            return nvxDeviceType
                .GetConstructor(new CType[] {typeof (ushort).GetCType(), typeof (CrestronControlSystem)})
                .Invoke(new object[] {props.Control.IpIdInt, Global.ControlSystem}) as DmNvxBaseClass;
        }

        private void BuildFeedbacks()
        {
            Feedbacks.Add(NvxDevice.DeviceFeedbacks.DeviceName,
                Device.GetDeviceNameFeedback(Config));

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.DeviceMode,
                Device.GetDeviceModeFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.DeviceStatus,
                Device.GetDeviceStatusFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.StreamUrl,
                Device.GetStreamUrlFeedback());

            Feedbacks.Add(NvxDevice.DeviceFeedbacks.MulticastAddress,
                Device.GetMulticastAddressFeedback());
        }

        private void BuildActions()
        {
            BoolActions.Add(NvxDevice.BoolActions.EnableVideoStream, enable =>
            {
                if (enable)
                    Device.Control.Start();
                else
                    Device.Control.Stop();
            });
        }

        protected abstract void BuildRoutingPorts(NvxDevice device);
    }
}