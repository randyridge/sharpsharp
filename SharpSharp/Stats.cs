using System;

namespace SharpSharp {
    public sealed class Stats {
        public ChannelStats[] ChannelStats { get; internal set; } = Array.Empty<ChannelStats>();

        public double Entropy { get; internal set; }

        public bool IsOpaque { get; internal set; }
    }
}
