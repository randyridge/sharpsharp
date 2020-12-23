using NetVips;

namespace SharpSharp {
	public sealed record ResizeOptions {
		public double[] Background { get; set; } = { 0.0, 0.0, 0.0, 255.0};

		public CoverBehavior Behavior { get; set; } = CoverBehavior.Center;

		public bool FastShrinkOnLoad { get; set; } = true;

		public Canvas Canvas { get; set;  } = Canvas.Crop;

		public int Height { get; set;  } = -1;

		public string Kernel { get; set; } = Enums.Kernel.Lanczos3;

		public Gravity Position { get; set; } = Gravity.Center;

		public int Width { get; set; } = -1;

		public bool WithoutEnlargement { get; set; } = false;
	}
}
