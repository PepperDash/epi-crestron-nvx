using System;
using System.Linq;
using Crestron.SimplSharp.Net;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public static class UsbStreamExt
    {
        public const string ClearUsbValue = "00:00:00:00:00:00";

        public static void AddRemoteUsbStreamToLocal(this IUsbStreamWithHardware local, IUsbStreamWithHardware remote)
        {
            try
            {
                if (local.IsRemote)
                    throw new NotSupportedException(local.Key);

                if (!remote.IsRemote)
                    throw new NotSupportedException(remote.Key);

                /*
                 * This doesn't work because we're comparing references, not values of the feedbacks
                if (local.UsbRemoteIds.ContainsValue(remote.UsbLocalId))
                    return;
                 */
                Debug.Console(1, remote, "Automatic Pairing is {0}.", remote.Hardware.UsbInput.AutomaticUsbPairingEnabledFeedback.BoolValue ? "Enabled" : "Disabled");

                Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);

                if (local.UsbRemoteIds.Any((x) => x.Value.StringValue.Equals(remote.UsbLocalId.StringValue)))
                {
                    var results =
                        DeviceManager.AllDevices.OfType<IUsbStreamWithHardware>()
                            .Where(x => !x.IsRemote)
                            .Where(
                                o =>
                                o.UsbRemoteIds.Any((x) => x.Value.StringValue.Equals(remote.UsbLocalId.StringValue))).ToList();

                    Debug.Console(2, "Found {0} Hosts with client {1} connected", results.Count(), remote.UsbLocalId);
                    foreach (var usb in results)
                    {
                        var localUsb = usb;
                        Debug.Console(2, "Clearing clients from {0}", localUsb.UsbLocalId);
                        if (localUsb.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                            localUsb.Hardware.UsbInput.RemovePairing();
                        localUsb.Hardware.UsbInput.RemoteDeviceId.StringValue = ClearUsbValue;
                        foreach (var remoteId in localUsb.UsbRemoteIds)
                        {
                            if (remoteId.Value == remote.UsbLocalId)
                            {
                                localUsb.Hardware.UsbInput.RemoteDeviceIds[remoteId.Key].StringValue = ClearUsbValue;
                            }
                        }
                        if(localUsb.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                            localUsb.Hardware.UsbInput.RemovePairing();
                    }
                    Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);

                    Debug.Console(2, remote, "Remote {0} already added to list. Setting remote to {1}",
                        remote.UsbLocalId, local.UsbLocalId);
                    remote.Hardware.UsbInput.RemoteDeviceId.StringValue = ClearUsbValue;
                    if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                        remote.Hardware.UsbInput.RemovePairing();
                    Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);

                    remote.Hardware.UsbInput.RemoteDeviceId.StringValue = local.UsbLocalId.StringValue;
                    local.Hardware.UsbInput.RemoteDeviceId.StringValue = remote.UsbLocalId.StringValue;
                    Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);
                    Debug.Console(2, remote, "There are {0} devices in RemoteIds", remote.Hardware.UsbInput.RemoteDeviceIds.Count);
                    foreach (var connection in remote.Hardware.UsbInput.RemoteDeviceIds)
                    {
                        Debug.Console(2, remote, connection.StringValue);
                    }
                    Debug.Console(2, local, "There are {0} devices in RemoteIds", local.Hardware.UsbInput.RemoteDeviceIds.Count);
                    foreach (var connection in local.Hardware.UsbInput.RemoteDeviceIds)
                    {
                        Debug.Console(2, local, connection.StringValue);
                    }
                    Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);
                    if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                        remote.Hardware.UsbInput.Pair();
                    if (local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                        local.Hardware.UsbInput.Pair();
                    Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);

                    return;
                }
                if (local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    local.Hardware.UsbInput.RemovePairing();
                if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    remote.Hardware.UsbInput.RemovePairing();
                Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);


                foreach (var remoteId in local.Hardware.UsbInput.RemoteDeviceIds)
                {
                    remoteId.StringValue = ClearUsbValue;
                }
                foreach (var remoteId in remote.Hardware.UsbInput.RemoteDeviceIds)
                {
                    remoteId.StringValue = ClearUsbValue;
                }

                Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);
                /*
                var indexLocal =
                    local.UsbRemoteIds.FirstOrDefault(
                        x => string.IsNullOrEmpty(x.Value.StringValue) || x.Value.StringValue.Equals(ClearUsbValue));

                var indexRemote =
                    remote.UsbRemoteIds.FirstOrDefault(
                        x => string.IsNullOrEmpty(x.Value.StringValue) || x.Value.StringValue.Equals(ClearUsbValue));

                if (indexLocal.Value == null)
                {
                    Debug.Console(0, remote, "Cannot pair to: {0}, it doesn't support any more connections", local.Key);
                    return;
                }
                if (indexRemote.Value == null)
                {
                    Debug.Console(0, remote, "Cannot pair to: {0}, it doesn't support any more connections", remote.Key);
                    return;
                }
                /*
                var localSig = local
                    .Hardware
                    .UsbInput
                    .RemoteDeviceIds[1];
                var remoteSig = remote
                    .Hardware
                    .UsbInput
                    .RemoteDeviceIds[1];

                if (localSig == null)
                {
                    Debug.Console(0, local, "Somehow local sig and index:{0} doesn't exist", indexLocal.Key);
                    return;
                }
                if (remoteSig == null)
                {
                    Debug.Console(0, remote, "Somehow remote sig and index:{0} doesn't exist", indexRemote.Key);
                    return;
                }
                 * */

                //Debug.Console(0, local, "Setting Remote Id: {0} to {1}", 1, remote.UsbLocalId.StringValue);
                Debug.Console(1, local, "Setting Remote Id to {0}", remote.UsbLocalId.StringValue);
                local.Hardware.UsbInput.RemoteDeviceId.StringValue = remote.UsbLocalId.StringValue;
                //localSig.StringValue = remote.UsbLocalId.StringValue;
                //Debug.Console(0, remote, "Setting Remote Id: {0} to {1}", 1, local.UsbLocalId.StringValue);
                Debug.Console(1, remote, "Setting Remote Id to {0}", local.UsbLocalId.StringValue);
                remote.Hardware.UsbInput.RemoteDeviceId.StringValue = local.UsbLocalId.StringValue;
                //remoteSig.StringValue = local.UsbLocalId.StringValue;
                Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(500);
              
                if (local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    local.Hardware.UsbInput.Pair();
                if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    remote.Hardware.UsbInput.Pair();
                Crestron.SimplSharpPro.CrestronThread.Thread.Sleep(1000);

            }
            catch (Exception ex)
            {
                Debug.Console(0, local, "Error adding remote stream to local : {0}", ex.Message);
            }   
        }
    }
}