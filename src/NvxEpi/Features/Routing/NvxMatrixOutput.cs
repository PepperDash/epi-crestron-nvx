using NvxEpi.Abstractions.Stream;
using NvxEpi.Devices;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NvxEpi.Features.Routing
{
    public class NvxMatrixOutput : IRoutingOutputSlot
    {       
        private readonly NvxBaseDevice _device;

        public NvxMatrixOutput(NvxBaseDevice device)
        {
            try
            {
                _device = device;

                var parent = NvxGlobalRouter.Instance;

                Debug.Console(0, this, $"Device is null: {_device == null}");

                _device.CurrentStreamId.OutputChange += (o, a) =>
                {
                    var inputSlot = parent.InputSlots.Values.FirstOrDefault(input => input.SlotNumber == a.IntValue);

                    SetInputRoute(eRoutingSignalType.Video, inputSlot);

                    if (!_device.IsStreamingSecondaryAudio.BoolValue)
                    {
                        SetInputRoute(eRoutingSignalType.Audio, inputSlot);
                    }
                };

                _device.CurrentSecondaryAudioStreamId.OutputChange += (o, a) =>
                {
                    var inputSlot = parent.InputSlots.Values.FirstOrDefault(input => input.SlotNumber == a.IntValue);

                    SetInputRoute(eRoutingSignalType.SecondaryAudio, inputSlot);
                };
            } catch (Exception ex)
            {
                Debug.Console(0, this, $"Exception creating NvxMatrixOutput: {ex.Message}");
            }
        }

        public string RxDeviceKey => _device.Key;

        private readonly Dictionary<eRoutingSignalType, IRoutingInputSlot> currentRoutes = new Dictionary<eRoutingSignalType, IRoutingInputSlot>
        {
            {eRoutingSignalType.Audio, null },
            {eRoutingSignalType.Video, null },
            {eRoutingSignalType.UsbInput, null },
            {eRoutingSignalType.UsbOutput, null },
            {eRoutingSignalType.SecondaryAudio, null },
        };

        public IStreamWithHardware Device => _device;

        private void SetInputRoute(eRoutingSignalType type, IRoutingInputSlot input)
        {
            if (currentRoutes.ContainsKey(type))
            {
                currentRoutes[type] = input;

                OutputSlotChanged?.Invoke(this, new EventArgs());

                return;
            }

            currentRoutes.Add(type, input);

            OutputSlotChanged?.Invoke(this, new EventArgs());
        }

        public Dictionary<eRoutingSignalType, IRoutingInputSlot> CurrentRoutes => currentRoutes;

        public int SlotNumber => _device.DeviceId;

        public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput | eRoutingSignalType.SecondaryAudio;

        public string Name => _device.Name;

        public string Key => $"{_device.Key}-matrixInput";

        public event EventHandler OutputSlotChanged;
    }
}
