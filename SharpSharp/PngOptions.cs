namespace SharpSharp {
	public sealed record PngOptions {
		public int Colors { get; set; } = 256,

		public int CompressionLevel { get; set; } = 9;

		public double Dither { get; set; } = 1.0;

		public bool MakeProgressive { get; set; } = false;

		public int Quality { get; set; } = 100;

		public bool UseAdaptiveFiltering { get; set; } = false;

		public bool UsePalette { get; set; } = false;
	}
}
