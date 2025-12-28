using System;
using System.Linq;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core;
using PepperDash.Core.Logging;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions;

public static class UsbStreamExt
{
    public const string ClearUsbValue = "00:00:00:00:00:00";

    /// <summary>
    /// Pairs a remote USB device with a local USB device.
    /// A local device can be linked to up to 7 remote devices, but a remote device can only have one local device.
    /// </summary>
    /// <param name="local">The local USB device (host)</param>
    /// <param name="remote">The remote USB device (client)</param>
    public static void AddRemoteUsbStreamToLocal(
        this IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote
    )
    {
        try
        {
            // A local device can be linked to up to 7 remote devices, but a remote device can only have one local device.
            if (local.IsRemote)
            {
                throw new NotSupportedException(
                    $"Local device parameter is actually remote: {local.Key}"
                );
            }

            ValidateDeviceTypes(local, remote);

            var remoteId = remote.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;
            var localId = local.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;

            remote.LogInformation(
                "Setting local device {localKey} RemoteDeviceId to local ID {remoteId}",
                local.Key,
                remoteId
            );

            local.Hardware.UsbInput.RemoteDeviceId.StringValue = remoteId;

            remote.LogInformation(
                "Setting remote device {remoteKey} RemoteDeviceId to local ID {localId}",
                remote.Key,
                localId
            );

            remote.Hardware.UsbInput.RemoteDeviceId.StringValue = localId;

            EstablishPairingIfManual(remote);
            EstablishPairingIfManual(local);
        }
        catch (Exception ex)
        {
            local.LogError(ex, "Error adding remote USB stream to local");
        }
    }

    /// <summary>
    /// Validates that the devices have the correct types (local vs remote)
    /// </summary>
    private static void ValidateDeviceTypes(
        IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote
    )
    {
        if (local.IsRemote)
            throw new NotSupportedException(
                $"Local device parameter is actually remote: {local.Key}"
            );

        if (!remote.IsRemote)
            throw new NotSupportedException(
                $"Remote device parameter is actually local: {remote.Key}"
            );
    }

    /// <summary>
    /// Gets the device IDs for both local and remote devices
    /// </summary>
    private static (string RemoteId, string LocalId) GetDeviceIds(
        IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote
    )
    {
        var remoteId = string.IsNullOrEmpty(
            remote.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue
        )
            ? ClearUsbValue
            : remote.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;

        var localId = string.IsNullOrEmpty(
            local.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue
        )
            ? ClearUsbValue
            : local.Hardware.UsbInput.LocalDeviceIdFeedback.StringValue;

        var remoteRemoteIds = remote.Hardware.UsbInput.RemoteDeviceIdFeedbacks;
        var localRemoteIds = local.Hardware.UsbInput.RemoteDeviceIdFeedbacks;

        remote.LogDebug(
            "Remote device IDS: {@remoteIds}",
            remoteRemoteIds.Values.Select(x => x.StringValue)
        );

        local.LogDebug(
            "Remote device IDS: {@localIds}",
            localRemoteIds.Values.Select(x => x.StringValue)
        );

        return (remoteId, localId);
    }

    /// <summary>
    /// Checks if the remote device is already paired with another local device
    /// </summary>
    private static bool IsRemoteAlreadyPairedWithLocal(IUsbStreamWithHardware remote)
    {
        remote.LogDebug("Checking if remote is already paired with a local device");

        return remote.Hardware.UsbInput.RemoteDeviceIdFeedbacks.Values.Any(x =>
        {
            remote.LogDebug(
                "Checking remote ID {remoteId} against local ID {localId}",
                x.StringValue,
                ClearUsbValue
            );
            return !x.StringValue.Equals(ClearUsbValue);
        });
    }

    /// <summary>
    /// Handles the case where the remote device is already paired with some local device
    /// </summary>
    private static void HandleExistingPairing(
        IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote,
        (string RemoteId, string LocalId) deviceIds
    )
    {
        remote.LogInformation(
            "Handling existing pairing for remote ID {remoteId} and local ID {localId}",
            deviceIds.RemoteId,
            deviceIds.LocalId
        );

        remote.LogInformation(
            "Current remoteIDs: {@remoteIds}",
            remote
                .UsbRemoteIds.Values.Select(x => x.StringValue)
                .Aggregate("", (current, next) => current + next + "; ")
        );

        local.LogInformation("Current localId: {@localId}", local.UsbLocalId.StringValue);

        ClearExistingPairingsForRemote(remote, deviceIds.RemoteId);
        SetupDirectPairing(local, remote, deviceIds);
    }

    /// <summary>
    /// Clears existing pairings for the remote device from all local devices
    /// </summary>
    private static void ClearExistingPairingsForRemote(
        IUsbStreamWithHardware remote,
        string remoteId
    )
    {
        remote.LogInformation("Clearing existing pairings for remote ID {remoteId}", remoteId);

        var pairedLocalDevices = DeviceManager
            .AllDevices.OfType<IUsbStreamWithHardware>()
            .Where(x =>
                !x.IsRemote
                && x.Hardware.UsbInput.RemoteDeviceIdFeedbacks.Values.Any(y =>
                    y.StringValue.Equals(remoteId)
                )
            )
            .ToList();

        remote.LogInformation(
            "Found {hostCount} Hosts with client {remoteId} connected",
            pairedLocalDevices.Count,
            remoteId
        );

        foreach (var localDevice in pairedLocalDevices)
        {
            remote.LogInformation("Clearing clients from {localId}", localDevice.UsbLocalId);

            RemovePairingIfManual(localDevice);
            ClearMatchingRemoteIds(localDevice, remoteId);
        }
    }

    /// <summary>
    /// Sets up direct pairing between local and remote devices
    /// </summary>
    private static void SetupDirectPairing(
        IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote,
        (string RemoteId, string LocalId) deviceIds
    )
    {
        remote.LogVerbose(
            "Remote {remoteId} already added to list. Setting remote to {localId}",
            deviceIds.RemoteId,
            deviceIds.LocalId
        );

        RemovePairingIfManual(remote);

        // Set the remote device to point to the local device
        remote.Hardware.UsbInput.RemoteDeviceId.StringValue = local.UsbLocalId.StringValue;

        EstablishPairingIfManual(remote);
        EstablishPairingIfManual(local);
    }

    /// <summary>
    /// Establishes a new pairing between local and remote devices
    /// </summary>
    private static void EstablishNewPairing(
        IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote,
        (string RemoteId, string LocalId) deviceIds
    )
    {
        RemovePairingIfManual(local);
        RemovePairingIfManual(remote);

        ClearAllRemoteDeviceIds(local);
        ClearAllRemoteDeviceIds(remote);

        SetupBidirectionalPairing(local, remote, deviceIds);

        EstablishPairingIfManual(local);
        EstablishPairingIfManual(remote);
    }

    /// <summary>
    /// Removes pairing if automatic pairing is disabled (manual mode)
    /// </summary>
    private static void RemovePairingIfManual(IUsbStreamWithHardware device)
    {
        if (device.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
        {
            device.Hardware.UsbInput.RemovePairing();
        }
    }

    /// <summary>
    /// Establishes pairing if automatic pairing is disabled (manual mode)
    /// </summary>
    private static void EstablishPairingIfManual(IUsbStreamWithHardware device)
    {
        if (device.Hardware.UsbInput.AutomaticUsbPairingDisabledFeedback.BoolValue)
        {
            device.Hardware.UsbInput.Pair();
        }
    }

    /// <summary>
    /// Clears remote device IDs that match the specified remote ID
    /// </summary>
    private static void ClearMatchingRemoteIds(IUsbStreamWithHardware localDevice, string remoteId)
    {
        var matchingIds = localDevice.Hardware.UsbInput.RemoteDeviceIds.Values.Where(sig =>
            sig.StringValue.Equals(remoteId)
        );

        foreach (var usbRemoteId in matchingIds)
        {
            usbRemoteId.StringValue = ClearUsbValue;
        }
    }

    /// <summary>
    /// Clears all remote device IDs for the specified device
    /// </summary>
    private static void ClearAllRemoteDeviceIds(IUsbStreamWithHardware device)
    {
        foreach (var id in device.Hardware.UsbInput.RemoteDeviceIds)
        {
            id.StringValue = ClearUsbValue;
        }
    }

    /// <summary>
    /// Sets up bidirectional pairing between local and remote devices
    /// </summary>
    private static void SetupBidirectionalPairing(
        IUsbStreamWithHardware local,
        IUsbStreamWithHardware remote,
        (string RemoteId, string LocalId) deviceIds
    )
    {
        local.LogDebug("Setting Remote Id to {remoteId}", deviceIds.RemoteId);
        local.Hardware.UsbInput.RemoteDeviceIds[1].StringValue = remote.UsbLocalId.StringValue;

        remote.LogDebug("Setting Remote Id to {localId}", deviceIds.LocalId);
        remote.Hardware.UsbInput.RemoteDeviceIds[1].StringValue = local.UsbLocalId.StringValue;
    }
}
