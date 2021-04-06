using System;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM.Streaming;
using NvxEpi.Abstractions;
using NvxEpi.Abstractions.HdmiOutput;
using NvxEpi.Abstractions.Usb;
using NvxEpi.Entities.Config;
using NvxEpi.Services.Bridge;
using NvxEpi.Services.InputPorts;
using NvxEpi.Services.InputSwitching;
using NvxEpi.Services.Utilities;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi.Aggregates
{
    public class NvxD3X : NvxBaseDevice, INvxD3XDeviceWithHardware, IComPorts, IIROutputPorts,
        IHdmiOutput,
        IRouting
    {
        private readonly DmNvxD3x _hardware;
        private readonly IHdmiOutput _hdmiOutput;
        private readonly IUsbStream _usbStream;

        public NvxD3X(DeviceConfig config, DmNvxD3x hardware)
            : base(config, hardware)
        {
            var props = NvxDeviceProperties.FromDeviceConfig(config);
            _hardware = hardware;
            _hdmiOutput = new NvxEpi.Entities.Hdmi.Output.HdmiOutput(this);

            RegisterForOnlineFeedback(hardware, props);
            RegisterForFeedback();
            AddRoutingPorts();
        }

        public CrestronCollection<ComPort> ComPorts
        {
            get { return Hardware.ComPorts; }
        }

        public new DmNvxD3x Hardware
        {
            get { return _hardware; }
        }

        public CrestronCollection<IROutputPort> IROutputPorts
        {
            get { return Hardware.IROutputPorts; }
        }

        public bool IsRemote
        {
            get { return _usbStream.IsRemote; }
        }

        public int NumberOfComPorts
        {
            get { return Hardware.NumberOfComPorts; }
        }

        public int NumberOfIROutputPorts
        {
            get { return Hardware.NumberOfIROutputPorts; }
        }

        public void ExecuteSwitch(object inputSelector, object outputSelector, eRoutingSignalType signalType)
        {
            try
            {
                var switcher = outputSelector as IHandleInputSwitch;
                if (switcher == null)
                    throw new NullReferenceException("outputSelector");

                Debug.Console(1,
                    this,
                    "Executing switch : '{0}' | '{1}' | '{2}'",
                    inputSelector.ToString(),
                    outputSelector.ToString(),
                    signalType.ToString());

                switcher.HandleSwitch(inputSelector, signalType);
            }
            catch (Exception ex)
            {
                Debug.Console(1, this, "Error executing switch! : {0}", ex.Message);
            }
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var deviceBridge = new NvxDeviceBridge(this);
            deviceBridge.LinkToApi(trilist, joinStart, joinMapKey, bridge);
        }

        private void AddRoutingPorts()
        {
            SwitcherForHdmiOutput.AddRoutingPort(this);
            StreamInput.AddRoutingPort(this);
            SecondaryAudioInput.AddRoutingPort(this);
            SwitcherForAnalogAudioOutput.AddRoutingPort(this);
        }

        private void RegisterForFeedback()
        {
            DeviceDebug.RegisterForDeviceFeedback(this);
            DeviceDebug.RegisterForPluginFeedback(this);
        }

        private void RegisterForOnlineFeedback(GenericBase hardware, NvxDeviceProperties props)
        {
            hardware.OnlineStatusChange += (device, args) =>
                {
                    if (!args.DeviceOnLine)
                        return;

                    Hardware.SetDefaults(props);
                };
        }

        public override bool IsTransmitter
        {
            get { return true; }
        }

        public BoolFeedback DisabledByHdcp
        {
            get { return _hdmiOutput.DisabledByHdcp; }
        }

        public IntFeedback HorizontalResolution
        {
            get { return _hdmiOutput.HorizontalResolution; }
        }
    }
}