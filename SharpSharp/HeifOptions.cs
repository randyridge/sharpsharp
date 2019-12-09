using RandyRidge.Common;

namespace SharpSharp {
    public sealed class HeifOptions {
        public HeifOptions(int quality = 80, HeifCompression heifCompression = HeifCompression.Hevc, bool useLossless = false) {
            Quality = Guard.RangeInclusive(quality, 1, 100, nameof(quality));
            Compression = heifCompression;
            UseLossless = useLossless;
        }

        public HeifCompression Compression { get; }

        public int Quality { get; }

        public bool UseLossless { get; }
    }
}
