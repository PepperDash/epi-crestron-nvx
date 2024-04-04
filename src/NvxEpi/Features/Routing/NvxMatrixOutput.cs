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
    public class NvxMatrixOutput<TInput> : RoutingOutputSlotBase<TInput> where TInput: NvxMatrixInput
    {       
        private readonly NvxBaseDevice _device;

        public NvxMatrixOutput(NvxBaseDevice device):base()
        {
            try
            {
                _device = device;

                _device.CurrentStreamId.OutputChange += OnPrimaryOutputChange;

                _device.CurrentSecondaryAudioStreamId.OutputChange += (o, a) =>
                {
                    
                };
            } catch (Exception ex)
            {
                Debug.LogMessage(ex, "Exception creating NvxMatrixOuput {ex}", this, ex.Message);                
            }
        }

        public override string RxDeviceKey => _device.Key;

        private readonly Dictionary<eRoutingSignalType, TInput> currentRoutes = new Dictionary<eRoutingSignalType, TInput>
        {
            {eRoutingSignalType.Audio, default },
            {eRoutingSignalType.Video, default },
            {eRoutingSignalType.UsbInput, default },
            {eRoutingSignalType.UsbOutput, default },
            {eRoutingSignalType.SecondaryAudio, default },
        };

        public IStreamWithHardware Device => _device;

        private void OnPrimaryOutputChange(object sender, FeedbackEventArgs args)
        {
            var parent = NvxGlobalRouter.Instance;

            var inputSlot = parent.InputSlots.Values.FirstOrDefault(input => input.SlotNumber == args.IntValue);

            SetInputRoute(eRoutingSignalType.Video, inputSlot as TInput);

            if (!_device.IsStreamingSecondaryAudio.BoolValue)
            {
                SetInputRoute(eRoutingSignalType.Audio, inputSlot as TInput);
            }
        }

        private void OnSecondaryAudioOutputChange(object sender, FeedbackEventArgs args)
        {
            var parent = NvxGlobalRouter.Instance;

            var inputSlot = parent.InputSlots.Values.FirstOrDefault(input => input.SlotNumber == args.IntValue);

            SetInputRoute(eRoutingSignalType.SecondaryAudio, inputSlot as TInput);
        }

        private void SetInputRoute(eRoutingSignalType type, TInput input)
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

        public override Dictionary<eRoutingSignalType, TInput> CurrentRoutes => currentRoutes;

        public override int SlotNumber => _device.DeviceId;

        public override eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.UsbInput | eRoutingSignalType.UsbOutput | eRoutingSignalType.SecondaryAudio;

        public override string Name => _device.Name;

        public override string Key => $"{_device.Key}-matrixInput";

        public override event EventHandler OutputSlotChanged;
    }
}
