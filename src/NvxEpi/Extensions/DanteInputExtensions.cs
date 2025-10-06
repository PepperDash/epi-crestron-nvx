using System;
using System.Globalization;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions.InputSwitching;
using PepperDash.Core;
using PepperDash.Core.Logging;

namespace NvxEpi.Extensions;

public static class DanteInputExtensions
{
    public static void SetDanteInput(this ICurrentDanteInput device, ushort input)
    {
        var inputToSwitch = (DmNvxControl.eAudioSource)input;

        switch (inputToSwitch)
        {
            case DmNvxControl.eAudioSource.Automatic:
                device.SetAudioToInputAutomatic();
                break;
            case DmNvxControl.eAudioSource.Input1:
                device.SetAudioToHdmiInput1();
                break;
            case DmNvxControl.eAudioSource.Input2:
                device.SetAudioToHdmiInput2();
                break;
            case DmNvxControl.eAudioSource.AnalogAudio:
                device.SetAudioToInputAnalog();
                break;
            case DmNvxControl.eAudioSource.PrimaryStreamAudio:
                device.SetAudioToPrimaryStreamAudio();
                break;
            case DmNvxControl.eAudioSource.DmNaxAudio:
                device.SetAudioToSecondaryStreamAudio();
                break;
            case DmNvxControl.eAudioSource.DanteAes67Audio:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(input.ToString(CultureInfo.InvariantCulture));
        }
    }

    public static void SetAudioToHdmiInput1(this ICurrentDanteInput device)
    {
        device.LogDebug("Switching Dante Input to : 'Hdmi1'");
        device.Hardware.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input1;
    }

    public static void SetAudioToHdmiInput2(this ICurrentDanteInput device)
    {
        device.LogDebug("Switching Dante Input to : 'Hdmi2'");
        device.Hardware.Control.DanteAudioSource = DmNvxControl.eAudioSource.Input2;
    }

    public static void SetAudioToInputAnalog(this ICurrentDanteInput device)
    {
        device.LogDebug("Switching Dante Input to : 'Analog'");
        device.Hardware.Control.DanteAudioSource = DmNvxControl.eAudioSource.AnalogAudio;
    }

    public static void SetAudioToPrimaryStreamAudio(this ICurrentDanteInput device)
    {
        device.LogDebug("Switching Dante Input to : 'PrimaryStream'");
        device.Hardware.Control.DanteAudioSource = DmNvxControl.eAudioSource.PrimaryStreamAudio;
    }

    public static void SetAudioToSecondaryStreamAudio(this ICurrentDanteInput device)
    {
        device.LogDebug("Switching Dante Input to : 'SecondaryStream'");
        device.Hardware.Control.DanteAudioSource = DmNvxControl.eAudioSource.DmNaxAudio;
    }

    public static void SetAudioToInputAutomatic(this ICurrentDanteInput device)
    {
        device.LogDebug("Switching Dante Input to : 'Automatic'");
        device.Hardware.Control.DanteAudioSource = DmNvxControl.eAudioSource.Automatic;
    }
}