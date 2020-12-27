namespace SharpSharp {
	public sealed record JpegOptions {
		public bool ApplyOvershootDeringing { get; set; } = false;

		public bool ApplyTrellisQuantization { get; set; } = false;

		public bool MakeProgressive { get; set; } = false;

		public bool OptimizeCoding { get; set; } = true;

		public bool OptimizeScans { get; set; } = false;

		public int Quality { get; set; } = 80;

		public int QuantizationTable { get; set; } = 0;

		public string Subsampling { get; set; } = "4:2:0";
	}
}
