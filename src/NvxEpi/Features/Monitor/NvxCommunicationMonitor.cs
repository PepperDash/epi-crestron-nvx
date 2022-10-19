using Crestron.SimplSharpPro;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Features.Monitor
{
    public class NvxCommunicationMonitor : StatusMonitorBase
    {
        private readonly IKeyed _parent;
        public readonly GenericBase Hardware;

        public NvxCommunicationMonitor(IKeyed parent, long warningTime, long errorTime, GenericBase hardware) : base(parent, warningTime, errorTime)
        {
            _parent = parent;
            Hardware = hardware;
            hardware.OnlineStatusChange += (device, args) => CheckIfDeviceIsOnlineAndUpdate();
            StatusChange += (sender, args) => Debug.Console(1, parent, Debug.ErrorLogLevel.Notice, "Essentials Online Status:{0}", Status.ToString());
            Status = MonitorStatus.StatusUnknown;
        }

        public override void Start()
        {
            Debug.Console(1, _parent, Debug.ErrorLogLevel.Notice, "Starting Monitor...");
            Status = MonitorStatus.StatusUnknown;
            CheckIfDeviceIsOnlineAndUpdate();
        }

        public override void Stop()
        {
            Debug.Console(1, _parent, Debug.ErrorLogLevel.Notice, "Stopping Monitor...");
            StopErrorTimers();
            Status = MonitorStatus.StatusUnknown;
        }

        public void CheckIfDeviceIsOnlineAndUpdate()
        {
            Debug.Console(1, _parent, Debug.ErrorLogLevel.Notice, "Checking if device is online:{0}", Hardware.IsOnline);
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