using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.CrestronThread;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Services.Messages;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Queues;

namespace NvxEpi.Devices
{
    public class NvxXioDirector : EssentialsDevice, INvxDirector, IOnline
    {
        private readonly BoolFeedback _isOnline;
        private readonly DmXioDirectorBase _hardware;

        private static IQueue<IQueueMessage> _queue;

        public NvxXioDirector(string key, string name, DmXioDirectorBase hardware) : base(key, name)
        {
            if (hardware == null)
                throw new ArgumentNullException("hardware");

            if (_queue == null)
                _queue = new GenericQueue("NvxDeviceBuildQueue", Thread.eThreadPriority.LowestPriority, 200);

            _hardware = hardware;
            _isOnline = new BoolFeedback("IsOnline", () => _hardware.IsOnline);
            _hardware.OnlineStatusChange += (device, args) => _isOnline.FireUpdate();

            _queue.Enqueue(new BuildNvxDeviceMessage(Key, _hardware));
        }

        public BoolFeedback IsOnline
        {
            get { return _isOnline; }
        }

        public DmXioDirectorBase Hardware
        {
            get { return _hardware; }
        }
    }
}