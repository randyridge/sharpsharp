namespace SharpSharp {
	public sealed record PreExtractionOptions {
		public int Height { get; set; } = -1;

		public int LeftOffset { get; set; } = -1;

		public int TopOffset { get; set; } = -1;

		public bool RotateBeforePreExtract { get; set; } = false;
		
		public int Width { get; set; } = -1;
	}
}
