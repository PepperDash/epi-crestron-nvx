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
            case 99:
                device.SetVideoToInputNone();
                break;
        }
    }

    public static void SetVideoToHdmiInput1(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Hdmi1'");
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi1;
    }

    public static void SetVideoToHdmiInput2(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Hdmi2'");
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Hdmi2;
    }

    public static void SetVideoToInputNone(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxD3x)
        {
            return;
        }
        device.LogDebug("Switching Video Input to : 'Disable'");
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Disable;
    }

    public static void SetVideoToStream(this ICurrentVideoInput device)
    {
        if (device.Hardware is DmNvxE3x || device.Hardware is DmNvxD3x || device.IsTransmitter)
        {
            return;
        }

        device.LogDebug("Switching Video Input to : 'Stream'");
        device.Hardware.Control.VideoSource = eSfpVideoSourceTypes.Stream;
    }

    public static void SetVideoToAutomatic(this ICurrentVideoInput device)
    {
        device.LogDebug("Switching Video Input to : 'Automatic'");
        device.Hardware.Control.EnableAutomaticInputRouting();
    }
}