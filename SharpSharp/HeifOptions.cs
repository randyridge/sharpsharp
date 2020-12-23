using RandyRidge.Common;

namespace SharpSharp {
	public sealed class HeifOptions {
		public HeifOptions(int quality = 50, HeifCompression heifCompression = HeifCompression.Av1, bool useLossless = false, int speed = 5) {
			Quality = Guard.RangeInclusive(quality, 1, 100, nameof(quality));
			Compression = heifCompression;
			Speed = speed;
			UseLossless = useLossless;
		}

		public HeifCompression Compression { get; }

		public int Quality { get; }

		public int Speed { get; }

		public bool UseLossless { get; }
	}
}
