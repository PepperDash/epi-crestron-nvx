using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Monitor
{
    public class NvxCommunicationMonitor : StatusMonitorBase
    {
        public readonly GenericBase Hardware;

        public NvxCommunicationMonitor(IKeyed parent, long warningTime, long errorTime, GenericBase hardware) : base(parent, warningTime, errorTime)
        {
            Hardware = hardware;
            hardware.OnlineStatusChange += (device, args) => CheckIfDeviceIsOnlineAndUpdate();
        }

        public override void Start()
        {
            CheckIfDeviceIsOnlineAndUpdate();
            IsOnlineFeedback.FireUpdate();
        }

        public override void Stop()
        {
            StopErrorTimers();
            Status = MonitorStatus.StatusUnknown;
        }

        public void CheckIfDeviceIsOnlineAndUpdate()
        {
            if (Hardware.IsOnline)
            {
                Status = MonitorStatus.IsOk;
                StopErrorTimers();
            }
            else
            {
                StartErrorTimers();
            }
        }
    }
}