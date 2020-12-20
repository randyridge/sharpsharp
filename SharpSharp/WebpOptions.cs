using RandyRidge.Common;

namespace SharpSharp {
	public sealed class WebpOptions {
		public WebpOptions(int quality = 80, int alphaQuality = 100, bool useLossless = false, bool useNearLossless = false, bool useSmartSubsample = false, int reductionEffort = 4) {
			Quality = Guard.RangeInclusive(quality, 1, 100, nameof(quality));
			AlphaQuality = Guard.RangeInclusive(alphaQuality, 0, 100, nameof(alphaQuality));
			UseLossless = useLossless;
			UseNearLossless = useNearLossless;
			UseSmartSubsample = useSmartSubsample;
			ReductionEffort = reductionEffort;
		}

		public int AlphaQuality { get; }

		public int Quality { get; }

		public int ReductionEffort { get; }

		public bool UseLossless { get; }

		public bool UseNearLossless { get; }

		public bool UseSmartSubsample { get; }
	}
}
