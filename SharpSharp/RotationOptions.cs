namespace SharpSharp {
	public sealed record RotationOptions {
		public int Angle { get; set; }

		public bool Flip { get; set; }

		public bool Flop { get; set; }

		public bool RotateBeforePreExtract { get; set; }

		public double RotationAngle { get; set; }

		public double[] RotationBackground { get; set; } = {0, 0, 0, 255};

		public bool UseExifOrientation { get; set; }
	}
}
