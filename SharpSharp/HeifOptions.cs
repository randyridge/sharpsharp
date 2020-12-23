using RandyRidge.Common;

namespace SharpSharp {
	public sealed class HeifOptions {
		public HeifOptions(int quality = 50, string heifCompression = "av1", bool useLossless = false, int speed = 5) {
			Quality = Guard.RangeInclusive(quality, 1, 100, nameof(quality));
			Compression = heifCompression;
			Speed = speed;
			UseLossless = useLossless;
		}

		public string Compression { get; }

		public int Quality { get; }

		public int Speed { get; }

		public bool UseLossless { get; }
	}
}
