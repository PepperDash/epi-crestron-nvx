using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.Stream;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public class StreamUtilities
    {
        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static Dictionary<int, IStream> _transmitters;

        public static IStream GetTxById(int txId)
        {
            try
            {
                _lock.Enter();
                if (_transmitters == null)
                {
                    _transmitters = DeviceManager
                        .AllDevices
                        .OfType<IStream>()
                        .Where(x => x.IsTransmitter)
                        .ToDictionary(x => x.DeviceId);
                }

                IStream tx;
                _transmitters.TryGetValue(txId, out tx);
                return tx;
            }
            finally
            {
                _lock.Leave();
            }
        }
    }
}