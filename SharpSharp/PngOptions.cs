using RandyRidge.Common;

namespace SharpSharp {
	public sealed class PngOptions {
		public PngOptions(int compressionLevel = 9, int quality = 100, bool makeProgressive = false,
			bool useAdaptiveFiltering = false, bool usePalette = false, int colors = 256, double dither = 1.0) {
			CompressionLevel = Guard.RangeInclusive(compressionLevel, 0, 9, nameof(compressionLevel));
			Quality = Guard.RangeInclusive(quality, 0, 100, nameof(quality));
			MakeProgressive = makeProgressive;
			UseAdaptiveFiltering = useAdaptiveFiltering;
			UsePalette = usePalette;
			Colors = colors;
			Dither = dither;
		}

		public int Colors { get; }

		public int CompressionLevel { get; }

		public double Dither { get; }

		public bool MakeProgressive { get; }

		public int Quality { get; }

		public bool UseAdaptiveFiltering { get; }

		public bool UsePalette { get; }
	}
}
