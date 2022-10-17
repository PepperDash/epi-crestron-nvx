using System;
using System.Linq;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Devices;
using NvxEpi.Features.Config;
using NvxEpi.Features.Routing;
using PepperDash.Essentials.Core;

namespace NvxEpi.Factories
{
    public abstract class NvxBaseDeviceFactory<T> : EssentialsPluginDeviceFactory<T> where T : EssentialsDevice
    {
        public const string MinumumEssentialsVersion = "1.9.0";

        static NvxBaseDeviceFactory()
        {
            if (DeviceManager.GetDeviceForKey(NvxGlobalRouter.InstanceKey) == null)
                DeviceManager.AddDevice(NvxGlobalRouter.Instance);
        }

        private static NvxXioDirector GetDirector(string key)
        {
            var result = DeviceManager.GetDeviceForKey(key) as NvxXioDirector;
            if (result == null)
                throw new NullReferenceException(key);

            return result;
        }

        protected static Func<DmNvxBaseClass> GetDeviceBuildAction(string type, NvxDeviceProperties props)
        {
            switch (type.ToLower())
            {
                case "dmnvx350":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx350(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx350((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx350((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx350c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx350C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx350C((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx350C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx351":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx351(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx351((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx351((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx351c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx351C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx351C((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx351C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx352":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx352(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx352((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx352((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx352c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx352C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx352C((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx352C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx360":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx360(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx360((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx360((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx360c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx360C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx360C((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx360C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx363":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx363(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx363((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx363((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvx363c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvx363C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvx363C((uint)props.DeviceId, domain, !props.DeviceIsTransmitter())
                                : new DmNvx363C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault(), !props.DeviceIsTransmitter());
                        };
                    }
                case "dmnvxd30":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxD30(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxD30((uint)props.DeviceId, domain)
                                : new DmNvxD30((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxd30c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxD30C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxD30C((uint)props.DeviceId, domain)
                                : new DmNvxD30C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxe30":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxE30(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxE30((uint)props.DeviceId, domain)
                                : new DmNvxE30((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxe30c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxE30C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxE30C((uint)props.DeviceId, domain)
                                : new DmNvxE30C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxe31":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxE31(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxE31((uint)props.DeviceId, domain)
                                : new DmNvxE31((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxe31c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxE31C(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxE31C((uint)props.DeviceId, domain)
                                : new DmNvxE31C((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxe760":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxE760(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxE760((uint)props.DeviceId, domain)
                                : new DmNvxE760((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                case "dmnvxe760c":
                    {
                        if (string.IsNullOrEmpty(props.ParentDeviceKey) ||
                            props.ParentDeviceKey.Equals("processor", StringComparison.OrdinalIgnoreCase))
                        {
                            return () => new DmNvxE760(props.Control.IpIdInt, Global.ControlSystem);
                        }
                        return () =>
                        {
                            var xio = GetDirector(props.ParentDeviceKey);
                            DmXioDirectorBase.DmXioDomain domain;

                            return xio.Hardware.Domain.TryGetValue(props.DomainId, out domain)
                                ? new DmNvxE760((uint)props.DeviceId, domain)
                                : new DmNvxE760((uint)props.DeviceId, xio.Hardware.Domain.Values.FirstOrDefault());
                        };
                    }
                default:
                    throw new NotSupportedException(type);
            }
        }
    }
}