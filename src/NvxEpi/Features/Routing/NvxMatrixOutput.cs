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
    public class NvxMatrixOutput : RoutingOutputSlotBase
    {       
        private readonly NvxBaseDevice _device;

        public NvxMatrixOutput(NvxBaseDevice device):base()
        {
            try
            {
                _device = device;

                var parent = NvxGlobalRouter.Instance;                

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
                Debug.LogMessage(ex, "Exception creating NvxMatrixOuput {ex}", this, ex.Message);                
            }
        }

        public override string RxDeviceKey => _device.Key;

        private readonly Dictionary<eRoutingSignalType, RoutingInputSlotBase> currentRoutes = new Dictionary<eRoutingSignalType, RoutingInputSlotBase>
        {
            {eRoutingSignalType.Audio, null },
            {eRoutingSignalType.Video, null },
            {eRoutingSignalType.UsbInput, null },
            {eRoutingSignalType.UsbOutput, null },
            {eRoutingSignalType.SecondaryAudio, null },
        };

        public IStreamWithHardware Device => _device;

        private void SetInputRoute(eRoutingSignalType type, RoutingInputSlotBase input)
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

        public override Dictionary<eRoutingSignalType, RoutingInputSlotBase> CurrentRoutes => currentRoutes;

        public override int SlotNumber => _device.DeviceId;

        public override eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput | eRoutingSignalType.SecondaryAudio;

        public override string Name => _device.Name;

        public override string Key => $"{_device.Key}-matrixInput";

        public override event EventHandler OutputSlotChanged;
    }
}
