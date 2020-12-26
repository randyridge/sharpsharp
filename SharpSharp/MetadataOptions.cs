namespace SharpSharp {
	public sealed record MetadataOptions {
		public string Icc { get; set; } = string.Empty;

		public int Orientation { get; set; } = -1;

		public bool ShouldStripMetadata => !WithMetadata;

		public bool WithMetadata { get; set; } = false;
	}
}
