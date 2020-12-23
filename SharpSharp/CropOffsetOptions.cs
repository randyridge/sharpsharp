namespace SharpSharp {
	public sealed record CropOffsetOptions {
		public bool HasCropOffset { get; set; } = false;
		
		public int Left { get; set; } = 0;

		public int Top { get; set; } = 0;
	}
}
