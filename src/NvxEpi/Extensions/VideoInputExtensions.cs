using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core.Logging;

namespace NvxEpi.Extensions;

public static class VideoInputExtensions
{
    //TODO: Documentation
    public static void SetVideoInput(this ICurrentVideoInput device, ushort input)
    {
        switch (input)
        {
            case 1:
                device.SetVideoToHdmiInput1();
                break;
            case 2:
                device.SetVideoToHdmiInput2();
                break;
            case 3:
                device.SetVideoToStream();
                break;
            case 4:
                device.SetVideoToAutomatic();
                break;
            case 11:
                device.SetVideoToUsbcInput1();
                break;
            case 12:
                device.SetVideoToUsbcInput2();
                break;
            case 99:
                device.SetVideoToInputNone();
                break;
        }
    }

    // The automatic-input-routing sig is not populated on every model. Crestron returns its
    // internal NullSig sentinel for it, so Enable/DisableAutomaticInputRouting() throw rather
    // than no-opping. E760 has a single DM input and no input switching, so the concept does
    // not apply there at all.
    private static bool SupportsAutomaticInputRouting(ICurrentVideoInput device) =>
        device.Hardware is not DmNvxE760x
        && device.Hardware is not DmNvxE3x
        && device.Hardware is not DmNvxE20
        && device.Hardware is not DmNvxD3x;

    public static void SetVideoToHdmiInput1(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxE20 || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Hdmi1'");
        if (SupportsAutomaticInputRouting(device))
        {
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
    }

    public static void SetVideoToHdmiInput2(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxE20 || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Hdmi2'");
        if (SupportsAutomaticInputRouting(device))
        {
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
    }

    public static void SetVideoToUsbcInput1(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxE20 || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Usbc1'");
        if (SupportsAutomaticInputRouting(device))
        {
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Usbc1;
    }

    public static void SetVideoToUsbcInput2(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxE20 || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Usbc2'");
        if (SupportsAutomaticInputRouting(device))
        {
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Usbc2;
    }

    public static void SetVideoToInputNone(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxE20 || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Disable'");
        if (SupportsAutomaticInputRouting(device))
        {
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Disable;
    }

    public static void SetVideoToStream(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxE20 || device.Hardware is DmNvxD3x || device.IsTransmitter)
        {
            return;
        }

        device.LogDebug("Switching Video Input to : 'Stream'");
        if (SupportsAutomaticInputRouting(device))
        {
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
    }

    public static void SetVideoToAutomatic(this ICurrentVideoInput device)
    {
        if (!SupportsAutomaticInputRouting(device))
        {
            return;
        }

        device.LogDebug("Switching Video Input to : 'Automatic'");
        device.Hardware.Control.EnableAutomaticInputRouting();
    }

    public static void SetAutomaticInputRouting(this ICurrentVideoInput device, bool pressed)
    {
        // bridge join is a press/release digital signal; act once per press and ignore the release
        if (!pressed)
        {
            return;
        }

        if (!SupportsAutomaticInputRouting(device))
        {
            return;
        }

        if (device.Hardware.Control.EnableAutomaticInputRoutingFeedback.BoolValue)
        {
            device.LogDebug("Disabling Automatic Input Routing");
            device.Hardware.Control.DisableAutomaticInputRouting();
        }
        else
        {
            device.LogDebug("Enabling Automatic Input Routing");
            device.Hardware.Control.EnableAutomaticInputRouting();
        }
    }
}