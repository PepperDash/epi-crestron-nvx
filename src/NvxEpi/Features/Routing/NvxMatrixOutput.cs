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
    public class NvxMatrixOutput :IRoutingOutputSlot
    {       
        private readonly NvxBaseDevice _device;

        public NvxMatrixOutput(NvxBaseDevice device)
        {
            try
            {
                _device = device;

                _device.CurrentStreamId.OutputChange += OnPrimaryOutputChange;

                _device.CurrentSecondaryAudioStreamId.OutputChange += OnSecondaryAudioOutputChange;
            } catch (Exception ex)
            {
                Debug.LogMessage(ex, "Exception creating NvxMatrixOutput {ex}", this, ex.Message);                
            }
        }

        public string RxDeviceKey => _device.Key;

        private readonly Dictionary<eRoutingSignalType, IRoutingInputSlot> currentRoutes = new Dictionary<eRoutingSignalType, IRoutingInputSlot>
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

            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Video: Found input slot {inputSlot} for {inputNumber}", this, inputSlot?.Key ?? "null", args.IntValue);

            SetInputRoute(eRoutingSignalType.Video, inputSlot);

            if (!_device.IsStreamingSecondaryAudio.BoolValue)
            {
                SetInputRoute(eRoutingSignalType.Audio, inputSlot);
            }
        }

        private void OnSecondaryAudioOutputChange(object sender, FeedbackEventArgs args)
        {
            var parent = NvxGlobalRouter.Instance;

            var inputSlot = parent.InputSlots.Values.FirstOrDefault(input => input.SlotNumber == args.IntValue);

            Debug.LogMessage(Serilog.Events.LogEventLevel.Verbose, "Audio: Found input slot {inputSlot} for {inputNumber}", this, inputSlot?.Key ?? "null", args.IntValue);

            SetInputRoute(eRoutingSignalType.SecondaryAudio, inputSlot);
            SetInputRoute(eRoutingSignalType.Audio, inputSlot);
        }

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

        public eRoutingSignalType SupportedSignalTypes => eRoutingSignalType.AudioVideo | eRoutingSignalType.SecondaryAudio;

        public string Name => _device.Name;

        public string Key => $"{_device.Key}";

        public event EventHandler OutputSlotChanged;
    }
}
