namespace SharpSharp {
	public sealed record WebpOptions {
		public int AlphaQuality { get; set; } = 100;

		public int Quality { get; set; } = 80;

		public int ReductionEffort { get; set; } = 4;

		public bool UseLossless { get; set; } = false;

		public bool UseNearLossless { get; set; } = false;

		public bool UseSmartSubsample { get; set; } = false;
	}
}
