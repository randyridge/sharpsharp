namespace SharpSharp {
	public sealed record GifOptions {
		public bool OptimizeFrames { get; set; } = true;

		public bool OptimizeTransparency { get; set; } = true;
	}
}
