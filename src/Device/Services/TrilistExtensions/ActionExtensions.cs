using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro.DeviceSupport;
using NvxEpi.Device.JoinMaps;
using NvxEpi.Device.Models;
using PepperDash.Essentials.Core;
using Feedback = PepperDash.Essentials.Core.Feedback;

namespace NvxEpi.Device.Services.TrilistExtensions
{
    public static class ActionExtensions
    {
        public static BasicTriList BuildBoolActions(this BasicTriList trilist,
            Dictionary<NvxDevice.BoolActions, Action<bool>> actions, NvxDeviceJoinMap joinMap)
        {
            return trilist;
        }

        public static BasicTriList BuildIntActions(this BasicTriList trilist,
            Dictionary<NvxDevice.IntActions, Action<ushort>> actions, NvxDeviceJoinMap joinMap)
        {
            foreach (var action in actions)
            {
                switch (action.Key)
                {
                    case NvxDevice.IntActions.VideoInputSelect:
                        trilist.SetUShortSigAction(joinMap.VideoInput.JoinNumber, action.Value);
                        break;
                    case NvxDevice.IntActions.AudioInputSelect:
                        trilist.SetUShortSigAction(joinMap.AudioInput.JoinNumber, action.Value);
                        break;
                    case NvxDevice.IntActions.NaxInputSelect:
                        break;
                    case NvxDevice.IntActions.Hdmi1HdcpCapability:
                        trilist.SetUShortSigAction(joinMap.Hdmi1Capability.JoinNumber, action.Value);
                        break;
                    case NvxDevice.IntActions.Hdmi2HdcpCapability:
                        trilist.SetUShortSigAction(joinMap.Hdmi2Capability.JoinNumber, action.Value);
                        break;
                    case NvxDevice.IntActions.VideowallMode:
                        trilist.SetUShortSigAction(joinMap.VideowallMode.JoinNumber, action.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return trilist;
        }

        public static BasicTriList BuildStringActions(this BasicTriList trilist,
            Dictionary<NvxDevice.StringActions, Action<string>> actions, NvxDeviceJoinMap joinMap)
        {
            foreach (var action in actions)
            {
                switch (action.Key)
                {
                    case NvxDevice.StringActions.StreamUrl:
                        trilist.SetStringSigAction(joinMap.StreamUrl.JoinNumber, action.Value);
                        break;
                    case NvxDevice.StringActions.SecondaryAudioAddress:
                        trilist.SetStringSigAction(joinMap.MulticastAudioAddress.JoinNumber, action.Value);
                        break;
                    case NvxDevice.StringActions.NaxTxAddress:
                        break;
                    case NvxDevice.StringActions.NaxRxAddress:
                        break;
                    case NvxDevice.StringActions.UsbRemoteId:
                        trilist.SetStringSigAction(joinMap.UsbRemoteId.JoinNumber, action.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return trilist;
        }
    }
}