using System;
using PepperDash.Essentials.Core;

namespace NvxEpi.JoinMaps
{
    public class Nvx38xMultiviewJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("MultiviewEnter")]
        public JoinDataComplete MultiviewEnter = new JoinDataComplete(
            new JoinData { JoinNumber = 200, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Enter Command", JoinCapabilities = (eJoinCapabilities)1, JoinType = (eJoinType)1 });

        [JoinName("MultiviewEnabledFeedback")]
        public JoinDataComplete MultiviewEnabledFeedback = new JoinDataComplete(
            new JoinData { JoinNumber = 201, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Enabled Feedback", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)1 });

        [JoinName("MultiviewDisabledFeedback")]
        public JoinDataComplete MultiviewDisabledFeedback = new JoinDataComplete(
            new JoinData { JoinNumber = 202, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Disabled Feedback", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)1 });

        [JoinName("MultiviewLayout")]
        public JoinDataComplete MultiviewLayout = new JoinDataComplete(
            new JoinData { JoinNumber = 210, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Layout Selection", JoinCapabilities = (eJoinCapabilities)1, JoinType = (eJoinType)2 });

        [JoinName("MultiviewAudioSource")]
        public JoinDataComplete MultiviewAudioSource = new JoinDataComplete(
            new JoinData { JoinNumber = 220, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Audio Source Selection", JoinCapabilities = (eJoinCapabilities)1, JoinType = (eJoinType)2 });

        // Stream URL joins for multiview windows (up to 6 windows)
        [JoinName("MultiviewWindow1StreamUrl")]
        public JoinDataComplete MultiviewWindow1StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 301, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 1 Stream URL", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow2StreamUrl")]
        public JoinDataComplete MultiviewWindow2StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 302, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 2 Stream URL", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow3StreamUrl")]
        public JoinDataComplete MultiviewWindow3StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 303, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 3 Stream URL", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow4StreamUrl")]
        public JoinDataComplete MultiviewWindow4StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 304, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 4 Stream URL", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow5StreamUrl")]
        public JoinDataComplete MultiviewWindow5StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 305, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 5 Stream URL", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow6StreamUrl")]
        public JoinDataComplete MultiviewWindow6StreamUrl = new JoinDataComplete(
            new JoinData { JoinNumber = 306, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 6 Stream URL", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        // Label joins for multiview windows (up to 6 windows)
        [JoinName("MultiviewWindow1Label")]
        public JoinDataComplete MultiviewWindow1Label = new JoinDataComplete(
            new JoinData { JoinNumber = 401, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 1 Label", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow2Label")]
        public JoinDataComplete MultiviewWindow2Label = new JoinDataComplete(
            new JoinData { JoinNumber = 402, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 2 Label", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow3Label")]
        public JoinDataComplete MultiviewWindow3Label = new JoinDataComplete(
            new JoinData { JoinNumber = 403, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 3 Label", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow4Label")]
        public JoinDataComplete MultiviewWindow4Label = new JoinDataComplete(
            new JoinData { JoinNumber = 404, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 4 Label", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow5Label")]
        public JoinDataComplete MultiviewWindow5Label = new JoinDataComplete(
            new JoinData { JoinNumber = 405, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 5 Label", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        [JoinName("MultiviewWindow6Label")]
        public JoinDataComplete MultiviewWindow6Label = new JoinDataComplete(
            new JoinData { JoinNumber = 406, JoinSpan = 1 },
            new JoinMetadata { Description = "Multiview Window 6 Label", JoinCapabilities = (eJoinCapabilities)2, JoinType = (eJoinType)3 });

        public Nvx38xMultiviewJoinMap(uint joinStart) : base(joinStart, typeof(Nvx38xMultiviewJoinMap)) { }
    }
}
