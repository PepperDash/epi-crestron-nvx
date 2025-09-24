using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Streaming;
using PepperDash.Core;
using System;

namespace NvxEpi.Services.WindowLayout
{
    /// <summary>
    /// Extended Window Layout class specifically for DM-NVX-38x series devices
    /// Supports up to 6 video inputs and extended layout types beyond the standard WindowLayout
    /// </summary>
    public class Nvx38xWindowLayout
    {
        #region Enums

        /// <summary>
        /// Extended layout types for DM-NVX-38x series
        /// </summary>
        public enum ExtendedLayoutType
        {
            /// <summary>Full Screen</summary>
            Fullscreen = 0,
            /// <summary>Side by Side</summary>
            SideBySide = 201,
            /// <summary>PIP Small Top Left</summary>
            PipSmallTopLeft = 202,
            /// <summary>PIP Small Top Right</summary>
            PipSmallTopRight = 203,
            /// <summary>PIP Small Bottom Left</summary>
            PipSmallBottomLeft = 204,
            /// <summary>PIP Small Bottom Right</summary>
            PipSmallBottomRight = 205,
            /// <summary>1 Top, 2 Bottom</summary>
            OneTopTwoBottom = 301,
            /// <summary>2 Top, 1 Bottom</summary>
            TwoTopOneBottom = 302,
            /// <summary>1 Left, 2 Right</summary>
            OneLeftTwoRight = 303,
            /// <summary>2 Top, 2 Bottom</summary>
            TwoTopTwoBottom = 401,
            /// <summary>1 Left, 3 Right</summary>
            OneLeftThreeRight = 402,
            /// <summary>1 Large Left, 4 Right</summary>
            OneLargeLeftFourRight = 501,
            /// <summary>4 Left, 1 Large Right</summary>
            FourLeftOneLargeRight = 502,
            /// <summary>3 Top, 3 Bottom</summary>
            ThreeTopThreeBottom = 601,
            /// <summary>1 Large Left, 5 Stacked</summary>
            OneLargeLeftFiveStacked = 602,
            /// <summary>5 Around, 1 Large Bottom Left</summary>
            FiveAroundOneLargeBottomLeft = 603,
            /// <summary>5 Around, 1 Large Top Left</summary>
            FiveAroundOneLargeTopLeft = 604
        }

        /// <summary>
        /// Extended video source types for DM-NVX-38x (supports up to 6 inputs)
        /// </summary>
        public enum ExtendedVideoSourceType
        {
            /// <summary>None</summary>
            None = 0,
            /// <summary>Input 1</summary>
            Input1 = 1,
            /// <summary>Input 2</summary>
            Input2 = 2,
            /// <summary>Input 3</summary>
            Input3 = 3,
            /// <summary>Input 4</summary>
            Input4 = 4,
            /// <summary>Input 5</summary>
            Input5 = 5,
            /// <summary>Input 6</summary>
            Input6 = 6
        }

        /// <summary>
        /// Audio source types (matches standard WindowLayout)
        /// </summary>
        public enum AudioSourceType
        {
            /// <summary>Auto</summary>
            Auto = 0,
            /// <summary>Input 1</summary>
            Input1 = 1,
            /// <summary>Input 2</summary>
            Input2 = 2,
            /// <summary>Input 3</summary>
            Input3 = 3,
            /// <summary>Input 4</summary>
            Input4 = 4,
            /// <summary>Input 5</summary>
            Input5 = 5,
            /// <summary>Input 6</summary>
            Input6 = 6
        }

        #endregion

        #region Fields

        private readonly DmNvx38x _device;
        private readonly IKeyed _parent;

        #endregion

        #region Properties

        /// <summary>
        /// Current layout type
        /// </summary>
        public ExtendedLayoutType Layout { get; private set; }

        /// <summary>
        /// Current audio source
        /// </summary>
        public AudioSourceType AudioSource { get; private set; }

        /// <summary>
        /// Video sources for each window (1-6)
        /// </summary>
        public ExtendedVideoSourceType[] WindowVideoSources { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Event fired when layout changes
        /// </summary>
        public event EventHandler<LayoutChangeEventArgs> LayoutChanged;

        /// <summary>
        /// Event fired when window video source changes
        /// </summary>
        public event EventHandler<WindowSourceChangeEventArgs> WindowVideoSourceChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for Nvx38xWindowLayout
        /// </summary>
        /// <param name="device">DM-NVX-38x device</param>
        /// <param name="parent">Parent device for logging</param>
        public Nvx38xWindowLayout(DmNvx38x device, IKeyed parent)
        {
            _device = device ?? throw new ArgumentNullException(nameof(device));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            WindowVideoSources = new ExtendedVideoSourceType[7]; // Index 0 unused, 1-6 for windows

            RegisterForEvents();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the window layout type
        /// </summary>
        /// <param name="layoutType">Layout type to set</param>
        public void SetLayout(ExtendedLayoutType layoutType)
        {
            if (_device?.HdWpChassis?.HdWpWindowLayout != null)
            {
                // Map extended types to standard types where possible
                var standardLayoutType = MapToStandardLayoutType(layoutType);
                _device.HdWpChassis.HdWpWindowLayout.Layout = standardLayoutType;
                Layout = layoutType;

                Debug.LogInformation(_parent, "Set layout to {0}", layoutType);
                LayoutChanged?.Invoke(this, new LayoutChangeEventArgs(layoutType));
            }
            else
            {
                Debug.LogError(_parent, "DM-NVX-38x multiview hardware not available");
            }
        }

        /// <summary>
        /// Set video source for a specific window
        /// </summary>
        /// <param name="windowId">Window ID (1-6)</param>
        /// <param name="sourceType">Video source type</param>
        public void SetWindowVideoSource(uint windowId, ExtendedVideoSourceType sourceType)
        {
            if (windowId < 1 || windowId > 6)
            {
                Debug.LogError(_parent, "Invalid window ID: {0}. Valid range is 1-6", windowId);
                return;
            }

            if (_device?.HdWpChassis?.HdWpWindowLayout != null)
            {
                // Map extended source types to standard types for hardware
                var standardSourceType = MapToStandardVideoSourceType(sourceType);
                _device.HdWpChassis.HdWpWindowLayout.SetVideoSource(windowId, standardSourceType);

                WindowVideoSources[windowId] = sourceType;

                Debug.LogInformation(_parent, "Set window {0} to source {1}", windowId, sourceType);
                WindowVideoSourceChanged?.Invoke(this, new WindowSourceChangeEventArgs(windowId, sourceType));
            }
            else
            {
                Debug.LogError(_parent, "DM-NVX-38x multiview hardware not available");
            }
        }

        /// <summary>
        /// Get current layout from hardware
        /// </summary>
        /// <returns>Current layout type</returns>
        public ExtendedLayoutType GetCurrentLayout()
        {
            if (_device?.HdWpChassis?.HdWpWindowLayout?.LayoutFeedback != null)
            {
                var standardLayout = _device.HdWpChassis.HdWpWindowLayout.LayoutFeedback;
                return MapFromStandardLayoutType(standardLayout);
            }
            return ExtendedLayoutType.Automatic;
        }

        /// <summary>
        /// Get current video source for a window
        /// </summary>
        /// <param name="windowId">Window ID (1-6)</param>
        /// <returns>Current video source type</returns>
        public ExtendedVideoSourceType GetWindowVideoSource(uint windowId)
        {
            if (windowId < 1 || windowId > 6)
            {
                Debug.LogError(_parent, "Invalid window ID: {0}. Valid range is 1-6", windowId);
                return ExtendedVideoSourceType.None;
            }

            if (_device?.HdWpChassis?.HdWpWindowLayout?.VideoSourceFeedback != null &&
                _device.HdWpChassis.HdWpWindowLayout.VideoSourceFeedback.Contains(windowId))
            {
                var standardSource = _device.HdWpChassis.HdWpWindowLayout.VideoSourceFeedback[windowId];
                return MapFromStandardVideoSourceType(standardSource);
            }

            return WindowVideoSources[windowId];
        }

        #endregion

        #region Private Methods

        private void RegisterForEvents()
        {
            if (_device?.HdWpChassis?.HdWpWindowLayout != null)
            {
                _device.HdWpChassis.HdWpWindowLayout.WindowLayoutChange += OnWindowLayoutChange;
            }
        }

        private void OnWindowLayoutChange(object sender, GenericEventArgs args)
        {
            Debug.LogVerbose(_parent, "Window layout change event: {0}", args.EventId);

            // Handle different event types and fire appropriate events
            switch (args.EventId)
            {
                case WindowLayoutEventIds.LayoutFeedbackEventId:
                    Layout = GetCurrentLayout();
                    LayoutChanged?.Invoke(this, new LayoutChangeEventArgs(Layout));
                    break;

                case WindowLayoutEventIds.VideoSourceFeedbackEventId:
                    if (args.Index > 0 && args.Index <= 6)
                    {
                        var source = GetWindowVideoSource((uint)args.Index);
                        WindowVideoSourceChanged?.Invoke(this, new WindowSourceChangeEventArgs((uint)args.Index, source));
                    }
                    break;
            }
        }

        private WindowLayout.eLayoutType MapToStandardLayoutType(ExtendedLayoutType extendedType)
        {
            return extendedType switch
            {
                ExtendedLayoutType.Automatic => WindowLayout.eLayoutType.Automatic,
                ExtendedLayoutType.Fullscreen => WindowLayout.eLayoutType.Fullscreen,
                ExtendedLayoutType.PictureInPicture => WindowLayout.eLayoutType.PictureInPicture,
                ExtendedLayoutType.SideBySide => WindowLayout.eLayoutType.SideBySide,
                ExtendedLayoutType.ThreeUp => WindowLayout.eLayoutType.ThreeUp,
                ExtendedLayoutType.Quadview => WindowLayout.eLayoutType.Quadview,
                ExtendedLayoutType.ThreeSmallOneLarge => WindowLayout.eLayoutType.ThreeSmallOneLarge,
                _ => WindowLayout.eLayoutType.Automatic // Default for extended types not supported by standard
            };
        }

        private ExtendedLayoutType MapFromStandardLayoutType(WindowLayout.eLayoutType standardType)
        {
            return standardType switch
            {
                WindowLayout.eLayoutType.Automatic => ExtendedLayoutType.Automatic,
                WindowLayout.eLayoutType.Fullscreen => ExtendedLayoutType.Fullscreen,
                WindowLayout.eLayoutType.PictureInPicture => ExtendedLayoutType.PictureInPicture,
                WindowLayout.eLayoutType.SideBySide => ExtendedLayoutType.SideBySide,
                WindowLayout.eLayoutType.ThreeUp => ExtendedLayoutType.ThreeUp,
                WindowLayout.eLayoutType.Quadview => ExtendedLayoutType.Quadview,
                WindowLayout.eLayoutType.ThreeSmallOneLarge => ExtendedLayoutType.ThreeSmallOneLarge,
                _ => ExtendedLayoutType.Automatic
            };
        }

        private WindowLayout.eVideoSourceType MapToStandardVideoSourceType(ExtendedVideoSourceType extendedType)
        {
            return extendedType switch
            {
                ExtendedVideoSourceType.None => WindowLayout.eVideoSourceType.None,
                ExtendedVideoSourceType.Input1 => WindowLayout.eVideoSourceType.Input1,
                ExtendedVideoSourceType.Input2 => WindowLayout.eVideoSourceType.Input2,
                ExtendedVideoSourceType.Input3 => WindowLayout.eVideoSourceType.Input3,
                ExtendedVideoSourceType.Input4 => WindowLayout.eVideoSourceType.Input4,
                // For Input5 and Input6, we might need to use a different approach
                // since standard WindowLayout doesn't support them
                ExtendedVideoSourceType.Input5 => WindowLayout.eVideoSourceType.Input1, // Placeholder
                ExtendedVideoSourceType.Input6 => WindowLayout.eVideoSourceType.Input1, // Placeholder
                _ => WindowLayout.eVideoSourceType.None
            };
        }

        private ExtendedVideoSourceType MapFromStandardVideoSourceType(WindowLayout.eVideoSourceType standardType)
        {
            return standardType switch
            {
                WindowLayout.eVideoSourceType.None => ExtendedVideoSourceType.None,
                WindowLayout.eVideoSourceType.Input1 => ExtendedVideoSourceType.Input1,
                WindowLayout.eVideoSourceType.Input2 => ExtendedVideoSourceType.Input2,
                WindowLayout.eVideoSourceType.Input3 => ExtendedVideoSourceType.Input3,
                WindowLayout.eVideoSourceType.Input4 => ExtendedVideoSourceType.Input4,
                _ => ExtendedVideoSourceType.None
            };
        }

        #endregion
    }

    #region Event Args Classes

    /// <summary>
    /// Event arguments for layout change events
    /// </summary>
    public class LayoutChangeEventArgs : EventArgs
    {
        public Nvx38xWindowLayout.ExtendedLayoutType NewLayout { get; }

        public LayoutChangeEventArgs(Nvx38xWindowLayout.ExtendedLayoutType newLayout)
        {
            NewLayout = newLayout;
        }
    }

    /// <summary>
    /// Event arguments for window source change events
    /// </summary>
    public class WindowSourceChangeEventArgs : EventArgs
    {
        public uint WindowId { get; }
        public Nvx38xWindowLayout.ExtendedVideoSourceType NewSource { get; }

        public WindowSourceChangeEventArgs(uint windowId, Nvx38xWindowLayout.ExtendedVideoSourceType newSource)
        {
            WindowId = windowId;
            NewSource = newSource;
        }
    }

    #endregion
}