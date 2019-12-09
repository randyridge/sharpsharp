using RandyRidge.Common;

namespace SharpSharp {
    public sealed class JpegOptions {
        public JpegOptions(int quality = 80, bool makeProgressive = true, bool useChromaSubsampling = true,
            bool applyTrellisQuantization = false, bool applyOvershootDeringing = false, bool optimizeScans = false,
            bool optimizeCoding = true, int quantizationTable = 0) {
            Quality = Guard.RangeInclusive(quality, 1, 100, nameof(quality));
            MakeProgressive = makeProgressive;
            UseChromaSubsampling = useChromaSubsampling;
            ApplyTrellisQuantization = applyTrellisQuantization;
            ApplyOvershootDeringing = applyOvershootDeringing;
            OptimizeScans = optimizeScans;
            OptimizeCoding = optimizeCoding;
            QuantizationTable = quantizationTable;
        }

        public bool ApplyOvershootDeringing { get; }

        public bool ApplyTrellisQuantization { get; }

        public bool MakeProgressive { get; }

        public bool NoSubsampling => !UseChromaSubsampling;

        public bool OptimizeCoding { get; }

        public bool OptimizeScans { get; }

        public int Quality { get; }

        public int QuantizationTable { get; }

        public bool UseChromaSubsampling { get; }
    }
}
