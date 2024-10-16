using System;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.Hardware;
using NvxEpi.Features.Config;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Devices;

public class NvxXioDirector : EssentialsDevice, INvxDirector, IOnline
{
    private readonly BoolFeedback _isOnline;
    private readonly DmXioDirectorBase _hardware;

    public NvxXioDirector(DeviceConfig config, DmXioDirectorBase hardware) : base(config.Key, config.Name)
    {
        var props = config.Properties.ToObject<NvxDirectorConfig>();

        for (var i = 1; i <= props.NumberOfDomains; i++)
        {
            if (hardware.Domain.Contains((uint)i))
            {
                continue;
            }

            var domain = new DmXioDirectorBase.DmXioDomain((uint)i, hardware);
            Debug.Console(1, this, "Adding domain:{0}", domain.Id);
            domain.DomainChange += (sender, args) => Debug.Console(1, this, "Domain {0} changed: {1}", domain.Id, args.EventId);
        }

        _hardware = hardware ?? throw new ArgumentNullException("hardware");
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