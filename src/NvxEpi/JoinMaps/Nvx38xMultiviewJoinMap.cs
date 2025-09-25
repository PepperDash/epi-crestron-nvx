using System;
using PepperDash.Essentials.Core;

namespace NvxEpi.JoinMaps
{
    public class Nvx38xMultiviewJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("MultiviewEnter")]
        public JoinDataComplete MultiviewEnter = new JoinDataComplete(
            new JoinData { JoinNumber = 200, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Enter - Select to update view, hold to always allow updates", JoinCapabilities = eJoinCapabilities.FromSIMPL, JoinType = eJoinType.Digital });

        [JoinName("MultiviewEnabledFeedback")]
        public JoinDataComplete MultiviewEnable = new JoinDataComplete(
            new JoinData { JoinNumber = 201, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Enable - Requires reboot, starts additional multicast stream", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Digital });

        [JoinName("MultiviewDisabledFeedback")]
        public JoinDataComplete MultiviewDisable = new JoinDataComplete(
            new JoinData { JoinNumber = 202, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Disable - Requires reboot, stops multiview stream", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Digital });

        [JoinName("MultiviewLayout")]
        public JoinDataComplete MultiviewLayout = new JoinDataComplete(
            new JoinData { JoinNumber = 210, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Layout", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Analog });

        [JoinName("MultiviewAudioSource")]
        public JoinDataComplete MultiviewAudioSource = new JoinDataComplete(
            new JoinData { JoinNumber = 220, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Audio Source - Choose window audio source, requires audio route set to 'multiview'", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Analog });

        // Stream URL joins for multiview windows (up to 6 windows)
        [JoinName("MultiviewWindow1StreamUrl")]
        public JoinDataComplete MultiviewWindow1StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 301, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 1 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow2StreamUrl")]
        public JoinDataComplete MultiviewWindow2StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 302, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 2 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow3StreamUrl")]
        public JoinDataComplete MultiviewWindow3StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 303, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 3 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow4StreamUrl")]
        public JoinDataComplete MultiviewWindow4StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 304, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 4 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow5StreamUrl")]
        public JoinDataComplete MultiviewWindow5StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 305, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 5 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow6StreamUrl")]
        public JoinDataComplete MultiviewWindow6StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 306, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 6 Stream URL", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        // Label joins for multiview windows (up to 6 windows)
        [JoinName("MultiviewWindow1Label")]
        public JoinDataComplete MultiviewWindow1Label = new JoinDataComplete(
            new JoinData { JoinNumber = 401, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 1 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow2Label")]
        public JoinDataComplete MultiviewWindow2Label = new JoinDataComplete(
            new JoinData { JoinNumber = 402, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 2 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow3Label")]
        public JoinDataComplete MultiviewWindow3Label = new JoinDataComplete(
            new JoinData { JoinNumber = 403, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 3 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow4Label")]
        public JoinDataComplete MultiviewWindow4Label = new JoinDataComplete(
            new JoinData { JoinNumber = 404, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 4 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow5Label")]
        public JoinDataComplete MultiviewWindow5Label = new JoinDataComplete(
            new JoinData { JoinNumber = 405, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 5 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        [JoinName("MultiviewWindow6Label")]
        public JoinDataComplete MultiviewWindow6Label = new JoinDataComplete(
            new JoinData { JoinNumber = 406, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 6 Label", JoinCapabilities = eJoinCapabilities.ToFromSIMPL, JoinType = eJoinType.Serial });

        public Nvx38xMultiviewJoinMap(uint joinStart) : base(joinStart, typeof(Nvx38xMultiviewJoinMap)) { }
    }
}
