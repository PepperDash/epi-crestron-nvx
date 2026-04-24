using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Features.Config;
using NvxEpi.Features.Monitor;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices;

public class NvxXioDirector : EssentialsDevice, INvxDirector, IOnline, ICommunicationMonitor
{
    private readonly BoolFeedback _isOnline;
    private readonly DmXioDirectorBase _hardware;
    public StatusMonitorBase CommunicationMonitor { get; private set; }

    public NvxXioDirector(DeviceConfig config, DmXioDirectorBase hardware) : base(config.Key, config.Name)
    {
        var props = config.Properties.ToObject<NvxDirectorConfig>();

        _hardware = hardware ?? throw new ArgumentNullException("hardware");

        for (var i = 1; i <= props.NumberOfDomains; i++)
        {
            if (_hardware.Domain.Contains((uint)i))
            {
                this.LogDebug("Domain {id} already exists, skipping add.", i);
                continue;
            }

            var domain = new DmXioDirectorBase.DmXioDomain((uint)i, _hardware);
            this.LogDebug("Adding domain: {id}", domain.Id);
        }

        _isOnline = new BoolFeedback("isOnline", () => _hardware.IsOnline);
        _hardware.OnlineStatusChange += (device, args) => _isOnline.FireUpdate();

        AddPreActivationAction(() => CommunicationMonitor = new NvxCommunicationMonitor(this, 10000, 30000, _hardware));
    }

    public override void Initialize()
    {
        this.LogDebug("Director configured with {count} domains out of {max}", _hardware.Domain.Count, _hardware.MaximumNumberOfDomains);

        foreach (var domain in _hardware.Domain)
        {
            this.LogDebug(" - Domain {id}", domain.Id);
            this.LogDebug("   - Total number of endpoints: {count}", domain.TotalNumberOfEndpoints);
            this.LogDebug("   - Inputs: {count}", domain.TotalNumberOfTransmitterEndpoints);
            this.LogDebug("   - Outputs: {count}", domain.TotalNumberOfReceiverEndpoints);
        }

        this.LogDebug("Registering NVX Director Hardware: {key}", Key);
        _hardware.RegisterWithLogging(Key);
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