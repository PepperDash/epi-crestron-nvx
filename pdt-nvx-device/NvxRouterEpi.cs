using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;

using NvxEpi.DeviceHelpers;
using NvxEpi.Interfaces;
using NvxEpi.Routing;

using PepperDash.Core;
using PepperDash.Essentials.Bridges;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Config;

namespace NvxEpi
{
    public class NvxRouterEpi : Device, IBridge
    {
        private readonly NvxRouterPropertiesConfig _config;
        public NvxRouterPropertiesConfig Config { get { return _config; } }

        private Dictionary<int, INvxDevice> _transmitters;
        private Dictionary<int, INvxDevice> _receivers;

        private Dictionary<uint, IntFeedback> _videoRoutes;
        private Dictionary<uint, IntFeedback> _audioRoutes;
        private Dictionary<uint, IntFeedback> _hdcpStates;
        private Dictionary<uint, StringFeedback> _inputNames;
        private Dictionary<uint, StringFeedback> _outputNames;
        private Dictionary<uint, StringFeedback> _currentVideoRoutes;
        private Dictionary<uint, StringFeedback> _currentAudioRoutes;
        private Dictionary<uint, IntFeedback> _outputHorizontalResolutions;
        private Dictionary<uint, BoolFeedback> _syncDetected;
        private Dictionary<uint, BoolFeedback> _inputsOnline;
        private Dictionary<uint, BoolFeedback> _outputsOnline;

        public IntFeedback GetVideoRouteFeedback(uint index)
        {
            IntFeedback feedback;
            if (_videoRoutes.TryGetValue(index, out feedback))
                return feedback;

            return new IntFeedback(() => default(int));
        }

        public IntFeedback GetAudioRouteFeedback(uint index)
        {
            IntFeedback feedback;
            if (_audioRoutes.TryGetValue(index, out feedback))
                return feedback;

            return new IntFeedback(() => default(int));
        }

        public StringFeedback GetInputNameFeedback(uint index)
        {
            StringFeedback feedback;
            if (_inputNames.TryGetValue(index, out feedback))
                return feedback;

            return new StringFeedback(() => _config.DeviceUnusedText ?? String.Empty);
        }

        public StringFeedback GetOutputNameFeedback(uint index)
        {
            StringFeedback feedback;
            if (_outputNames.TryGetValue(index, out feedback))
                return feedback;

            return new StringFeedback(() => _config.DeviceUnusedText ?? String.Empty);
        }

        public StringFeedback GetCurrentVideoRouteFeedback(uint index)
        {
            StringFeedback feedback;
            if (_currentVideoRoutes.TryGetValue(index, out feedback))
                return feedback;

            return new StringFeedback(() => "No Source");
        }

        public StringFeedback GetCurrentAudioRouteFeedback(uint index)
        {
            StringFeedback feedback;
            if (_currentAudioRoutes.TryGetValue(index, out feedback))
                return feedback;

            return new StringFeedback(() => "No Source");
        }

        public IntFeedback GetCurrentHorizontalResolutionFeedback(uint index)
        {
            IntFeedback feedback;
            if (_outputHorizontalResolutions.TryGetValue(index, out feedback))
                return feedback;

            return new IntFeedback(() => default(int));
        }

        public BoolFeedback GetHdmiSyncStatusFeedback(uint index)
        {
            BoolFeedback feedback;
            if (_syncDetected.TryGetValue(index, out feedback))
                return feedback;

            return new BoolFeedback(() => false);
        }

        public BoolFeedback GetTxOnlineFb(uint index)
        {
            BoolFeedback feedback;
            if (_inputsOnline.TryGetValue(index, out feedback))
                return feedback;

            return new BoolFeedback(() => false);
        }

        public BoolFeedback GetRxOnlineFb(uint index)
        {
            BoolFeedback feedback;
            if (_outputsOnline.TryGetValue(index, out feedback))
                return feedback;

            return new BoolFeedback(() => false);
        }

        public IntFeedback GetHdcpStateFb(uint index)
        {
            IntFeedback feedback;
            if (_hdcpStates.TryGetValue(index, out feedback))
                return feedback;

            return new IntFeedback(() => default(int));
        }

        public NvxRouterEpi(string key, string name, NvxRouterPropertiesConfig config)
            : base(key, name)
        {
            _config = config;

            AddPostActivationAction(() =>
                {
                    _transmitters = DeviceManager
                        .AllDevices
                        .OfType<INvxDevice>()
                        .Where(x => (x.ParentRouterKey.Equals(Key, StringComparison.OrdinalIgnoreCase) 
                            || x.ParentRouterKey.Equals(NvxDeviceEpi.DefaultRouterKey)) 
                            && x.IsTransmitter)
                        .ToDictionary(x => x.VirtualDevice);

                    _receivers = DeviceManager
                        .AllDevices
                        .OfType<INvxDevice>()
                        .Where(x => (x.ParentRouterKey.Equals(Key, StringComparison.OrdinalIgnoreCase)
                            || x.ParentRouterKey.Equals(NvxDeviceEpi.DefaultRouterKey))
                            && !x.IsTransmitter)
                        .ToDictionary(x => x.VirtualDevice);
                });

            AddPostActivationAction(() =>
                {
                    Debug.Console(2, this, "My transmitters...");
                    _transmitters.ToList().ForEach(tx => Debug.Console(2, this, "{0}", tx.Value.DeviceName));

                    Debug.Console(2, this, "My receivers...");
                    _receivers.ToList().ForEach(rx => Debug.Console(2, this, "{0}", rx.Value.DeviceName));
                });

            AddPostActivationAction(() =>
                {
                    SetupVideoRoutingFeedbacks();
                    SetupAudioRoutingFeedbacks();
                    SetupNameFeedbacks();
                    SetupCurrentRouteFeedbacks();
                    SetupOutputResolutions();
                    SetupSyncDetectedFeedbacks();
                    SetupDeviceOnlineFeedbacks();
                    SetupHdcpFeedbacks();
                });
        }

        private void SetupVideoRoutingFeedbacks()
        {
            _videoRoutes = _receivers.ToDictionary(x => (uint)x.Key, x => (IntFeedback)x.Value.VideoSourceFb);
        }

        private void SetupAudioRoutingFeedbacks()
        {
            _audioRoutes = _receivers.ToDictionary(x => (uint)x.Key, x => (IntFeedback)x.Value.AudioSourceFb);
        }

        private void SetupNameFeedbacks()
        {
            _inputNames = _transmitters.ToDictionary(x => (uint)x.Key, x => (StringFeedback)x.Value.DeviceNameFb);
            _outputNames = _receivers.ToDictionary(x => (uint)x.Key, x => (StringFeedback)x.Value.DeviceNameFb);
        }

        private void SetupCurrentRouteFeedbacks()
        {
            _currentVideoRoutes = _receivers.ToDictionary(x => (uint)x.Key, x => (StringFeedback)x.Value.CurrentlyRoutedVideoSourceFb);
            _currentAudioRoutes = _receivers.ToDictionary(x => (uint)x.Key, x => (StringFeedback)x.Value.CurrentlyRoutedAudioSourceFb);
        }

        private void SetupOutputResolutions()
        {
            _outputHorizontalResolutions = _receivers.ToDictionary(x => (uint)x.Key, x => (IntFeedback)x.Value.OutputResolutionFb);
        }

        private void SetupSyncDetectedFeedbacks()
        {
            _syncDetected = _receivers.ToDictionary(x => (uint)x.Key, x => (BoolFeedback)x.Value.HdmiInput1SyncDetectedFb);
        }

        private void SetupDeviceOnlineFeedbacks()
        {
            _inputsOnline = _transmitters.ToDictionary(x => (uint)x.Key, x => (BoolFeedback)x.Value.IsOnlineFb);
            _outputsOnline = _receivers.ToDictionary(x => (uint)x.Key, x => (BoolFeedback)x.Value.IsOnlineFb);
        }

        private void SetupHdcpFeedbacks()
        {
            _hdcpStates = _transmitters.ToDictionary(x => (uint)x.Key, x => (IntFeedback)x.Value.HdmiInput1HdmiCapabilityFb);
        }

        public void RouteVideo(int input, int output)
        {
            INvxDevice rx = null;
            if (!_receivers.TryGetValue(output, out rx)) 
                return;

            if (input == 0)
            {
                rx.VideoSource = 0;
                return;
            }
            
            INvxDevice tx = null;
            if (!_transmitters.TryGetValue(input, out tx)) 
                return;

            rx.VideoSource = tx.VirtualDevice;
        }

        public void RouteAudio(int input, int output)
        {
            INvxDevice rx = null;
            if (!_receivers.TryGetValue(output, out rx))
                return;

            if (input == 0)
            {
                rx.AudioSource = 0;
                return;
            }

            INvxDevice tx = null;
            if (!_transmitters.TryGetValue(input, out tx))
                return;

            rx.AudioSource = tx.VirtualDevice;
        }

        public void SetHdcpState(int input, int state)
        {
            INvxDevice tx = null;
            if (!_transmitters.TryGetValue(input, out tx))
                return;

            tx.HdmiInput1HdmiCapability = state;
        }

        #region IBridge Members

        public void LinkToApi(Crestron.SimplSharpPro.DeviceSupport.BasicTriList trilist, uint joinStart, string joinMapKey)
        {
            var bridge = new NvxRouterBridge(this, trilist, joinStart, joinMapKey);
            bridge.LinkToApi();
        }

        #endregion
    }
}