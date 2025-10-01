using System;
using System.Linq;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions;

public static class UsbStreamExt
{
    public const string ClearUsbValue = "00:00:00:00:00:00";

    public static void AddRemoteUsbStreamToLocal(this IUsbStreamWithHardware local, IUsbStreamWithHardware remote)
    {
        try
        {
            // A local device can be linked to up to 7 remote devices, but a remote device can only have one local device.

            if (local.IsRemote)
                throw new NotSupportedException(local.Key);

            if (!remote.IsRemote)
                throw new NotSupportedException(remote.Key);

            remote.LogDebug("Automatic Pairing is {automaticPairing}.", remote.Hardware.UsbInput.AutomaticUsbPairingEnabledFeedback.BoolValue ? "Enabled" : "Disabled");

            // Thread.Sleep(500);

            var remoteId = remote.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;
            var localId = local.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;

            var remoteRemoteIds = remote.Hardware.UsbInput.RemoteDeviceIds;
            var localRemoteIds = local.Hardware.UsbInput.RemoteDeviceIds;

            // Check if the remote id is already in the local's list of remote ids. A remote device can only have one local device, so if the remote id is already in the local's list, but not paired with it
            // we need to find what other local device(s) are paired with that remote device and clear the remote id from those local devices.
            if (localRemoteIds.Values.Any((x) => x.StringValue.Equals(remoteId)))
            {
                // Get a list of all devices that are local and are currently paired with the remote device
                var pairedLocalUsbDevices = DeviceManager.AllDevices
                    .OfType<IUsbStreamWithHardware>()
                    .Where(x => !x.IsRemote && x.Hardware.UsbInput.RemoteDeviceIds.Values.Any((y) => y.StringValue.Equals(remoteId))).ToList();

                remote.LogVerbose("Found {hostCount} Hosts with client {remoteId} connected", pairedLocalUsbDevices.Count, remoteId);

                // If we have a local device that has the same remote id as the one we're trying to add, we need to clear the remote device id from that local device
                foreach (var localUsb in pairedLocalUsbDevices)
                {
                    local.LogVerbose("Clearing clients from {localId}", localUsb.UsbLocalId);

                    if (localUsb.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    {
                        localUsb.Hardware.UsbInput.RemovePairing();
                    }

                    foreach (var usbRemoteId in localUsb.Hardware.UsbInput.RemoteDeviceIds.Values.Where(sig => sig.StringValue.Equals(remoteId)))
                    {
                        // Clear the remote device id if it matches the one we're trying to add

                        usbRemoteId.StringValue = ClearUsbValue;

                    }
                }

                // Thread.Sleep(500);

                remote.LogVerbose("Remote {remoteId} already added to list. Setting remote to {localId}",
                    remoteId, localId);

                if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    remote.Hardware.UsbInput.RemovePairing();

                // Thread.Sleep(500);

                // The remoteDeviceId sig is the equivalent of the RemoteDeviceIds[1]
                remote.Hardware.UsbInput.RemoteDeviceId.StringValue = local.UsbLocalId.StringValue;

                if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    remote.Hardware.UsbInput.Pair();
                if (local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                    local.Hardware.UsbInput.Pair();

                // Thread.Sleep(500);

                return;
            }

            if (local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                local.Hardware.UsbInput.RemovePairing();
            if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                remote.Hardware.UsbInput.RemovePairing();

            // Clear all remote device ids for both local and remote devices. This removes ALL existing pairings and allows us to set the new pairing.
            foreach (var id in local.Hardware.UsbInput.RemoteDeviceIds)
            {
                id.StringValue = ClearUsbValue;
            }

            foreach (var id in remote.Hardware.UsbInput.RemoteDeviceIds)
            {
                id.StringValue = ClearUsbValue;
            }

            // Thread.Sleep(500);

            local.LogDebug("Setting Remote Id to {remoteId}", remoteId);

            local.Hardware.UsbInput.RemoteDeviceId.StringValue = remote.UsbLocalId.StringValue;

            remote.LogDebug("Setting Remote Id to {localId}", localId);

            remote.Hardware.UsbInput.RemoteDeviceId.StringValue = local.UsbLocalId.StringValue;

            // Thread.Sleep(500);

            if (local.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                local.Hardware.UsbInput.Pair();
            if (remote.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
                remote.Hardware.UsbInput.Pair();
            // Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            local.LogError(ex, "Error adding remote USB stream to local");
        }
    }
}