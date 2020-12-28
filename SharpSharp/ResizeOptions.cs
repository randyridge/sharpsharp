using System;
using NetVips;

namespace SharpSharp {
	public sealed record ResizeOptions {
		public double[] AffineBackground { get; set; } = {0.0, 0.0, 0.0, 255.0};

		public int AffineIdx { get; set; } = 0;

		public int AffineIdy { get; set; } = 0;

		public Interpolate AffineInterpolator { get; set; } = Interpolators.Bilinear;

		public double[] AffineMatrix { get; set; } = Array.Empty<double>();

		public int AffineOdx { get; set; } = 0;

		public int AffineOdy { get; set; } = 0;

		public double[] Background { get; set; } = {0.0, 0.0, 0.0, 255.0};

		public Canvas Canvas { get; set; } = Canvas.Crop;

		public double[] ExtendBackground { get; set; } = {0.0, 0.0, 0.0, 255.0};

		public int ExtendBottom { get; set; } = 0;

		public int ExtendLeft { get; set; } = 0;

		public int ExtendRight { get; set; } = 0;

		public int ExtendTop { get; set; } = 0;

		public bool FastShrinkOnLoad { get; set; } = true;

		public int Height { get; set; } = -1;

		public int HeightPost { get; set; } = -1;

		public int HeightPre { get; set; } = -1;

		public string Kernel { get; set; } = Enums.Kernel.Lanczos3;

		public int LeftOffsetPost { get; set; } = -1;

		public int LeftOffsetPre { get; set; } = -1;

		public Gravity Position { get; set; } = Gravity.Center;

		public bool RotateBeforePreExtract { get; set; } = false;

		public int TopOffsetPost { get; set; } = -1;

		public int TopOffsetPre { get; set; } = -1;

		public int Width { get; set; } = -1;

		public int WidthPost { get; set; } = -1;

		public int WidthPre { get; set; } = -1;

		public bool WithoutEnlargement { get; set; } = false;
	}
}
