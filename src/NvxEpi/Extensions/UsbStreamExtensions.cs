using System;
using System.Linq;
using NvxEpi.Abstractions.Usb;
using PepperDash.Core;

namespace NvxEpi.Extensions
{
    public static class UsbStreamExt
    {
        public const string ClearUsbValue = "00:00:00:00:00";

        public static void AddRemoteUsbStreamToLocal(this IUsbStreamWithHardware local, IUsbStreamWithHardware remote)
        {
            if (local.IsRemote)
                throw new NotSupportedException(local.Key);

            if (!remote.IsRemote)
                throw new NotSupportedException(remote.Key);


            if (local.UsbRemoteIds.ContainsValue(remote.UsbLocalId))
                return;

            var index =
                local.UsbRemoteIds.FirstOrDefault(
                    x => string.IsNullOrEmpty(x.Value.StringValue) || x.Value.StringValue.Equals(ClearUsbValue));

            if (index.Value == null)
            {
                Debug.Console(1, remote, "Cannot pair to : {0}, it doesn't support any more connections", local.Key);
                return;
            }

            var inputSig = local
                .Hardware
                .UsbInput
                .RemoteDeviceIds[index.Key];

            if (inputSig == null)
            {
                Debug.Console(0, local, "Somehow input sig and index:{0} doesn't exist", index.Key);
                return;
            }

            Debug.Console(1, local, "Setting Remote Id : {0} to {1}", index, remote.UsbLocalId.StringValue);
            inputSig.StringValue = remote.UsbLocalId.StringValue;
            Debug.Console(1, remote, "Setting Remote Id to {0}", local.UsbLocalId.StringValue);
            remote.Hardware.UsbInput.RemoteDeviceId.StringValue = local.UsbLocalId.StringValue;
        }
    }
}