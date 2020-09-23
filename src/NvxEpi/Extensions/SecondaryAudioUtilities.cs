using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;
using NvxEpi.Abstractions.SecondaryAudio;
using PepperDash.Essentials.Core;

namespace NvxEpi.Extensions
{
    public class SecondaryAudioUtilities
    {
        private static readonly CCriticalSection _lock = new CCriticalSection();
        private static Dictionary<int, ISecondaryAudioStream> _transmitters;

        public static ISecondaryAudioStream GetTxById(int txId)
        {
            try
            {
                _lock.Enter();
                if (_transmitters == null)
                {
                    _transmitters = DeviceManager
                        .AllDevices
                        .OfType<ISecondaryAudioStream>()
                        .Where(x => x.IsTransmitter)
                        .ToDictionary(x => x.DeviceId);
                }

                ISecondaryAudioStream tx;
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