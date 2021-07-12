using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using PepperDash.Essentials.Core;

namespace NvxEpi.Devices
{
    public class NvxXioDirector : EssentialsDevice, INvxDirector, IOnline
    {
        private readonly BoolFeedback _isOnline;
        private readonly DmXioDirectorBase _hardware;

        public NvxXioDirector(string key, string name, DmXioDirectorBase hardware) : base(key, name)
        {
            if (hardware == null)
                throw new ArgumentNullException("hardware");

            _hardware = hardware;
            _isOnline = new BoolFeedback("BuildFeedbacks", () => _hardware.IsOnline);
            _hardware.OnlineStatusChange += (device, args) => _isOnline.FireUpdate();
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