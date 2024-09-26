using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Features.Monitor;
using PepperDash.Essentials.Core;

namespace NvxEpi.Devices;

public class NvxXioDirector : EssentialsDevice, INvxDirector, IOnline, ICommunicationMonitor
{
    private readonly BoolFeedback _isOnline;
    private readonly DmXioDirectorBase _hardware;

    public StatusMonitorBase CommunicationMonitor { get; private set; }

    public NvxXioDirector(string key, string name, DmXioDirectorBase hardware) : base(key, name)
    {
        _hardware = hardware ?? throw new ArgumentNullException("hardware");
        _isOnline = new BoolFeedback("BuildFeedbacks", () => _hardware.IsOnline);
        _hardware.OnlineStatusChange += (device, args) => _isOnline.FireUpdate();

        AddPreActivationAction(() => CommunicationMonitor = new NvxCommunicationMonitor(this, 10000, 30000, _hardware));
    }

    public override bool CustomActivate()
    {
        CommunicationMonitor.Start();

        return base.CustomActivate();
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