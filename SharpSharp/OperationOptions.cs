using System;

namespace SharpSharp {
	public sealed record OperationOptions {
		public string? BandBoolOp { get; set; } = null;

		public double BlurSigma { get; set; } = 0;

		public double Brightness { get; set; } = 1;

		public string ColorSpace { get; set; } = "srgb";
		
		public double[] Composite { get; set; } = Array.Empty<double>();

		public double[] ConvolveKernel { get; set; } = Array.Empty<double>();

		public int ConvolveKernelHeight { get; set; } = 0;

		public double ConvolveKernelOffset { get; set; } = 0;

		public double ConvolveKernelScale { get; set; } = 0;

		public int ConvolveKernelWidth { get; set; } = 0;

		public bool EnsureAlpha { get; set; } = false;
		
		public int ExtractChannel = -1;

		public bool Flatten { get; set; } = false;

		public double[] FlattenBackground { get; set; } = {0.0, 0.0, 0.0};

		public double Gamma { get; set; } = 0;

		public double GammaOut { get; set; } = 0;

		public bool Grayscale { get; set; } = false;

		public int Hue { get; set; } = 0;

		public double[] JoinChannelIn { get; set; } = Array.Empty<double>();

		public double LinearA { get; set; } = 1.0;

		public double LinearB { get; set; } = 0.0;

		public int MedianSize { get; set; } = 0;

		public bool Negate { get; set; } = false;

		public bool Normalize { get; set; } = false;

		public bool Premultiplied { get; set; } = false;

		public double[] RecombMatrix { get; set; } = Array.Empty<double>();

		public bool RemoveAlpha { get; set; } = false;

		public double Saturation { get; set; } = 1;

		public double SharpenFlat { get; set; } = 1;

		public double SharpenJagged { get; set; } = 2;

		public double SharpenSigma { get; set; } = 0;

		public double Threshold { get; set; } = 0.0;

		public bool ThresholdGrayscale { get; set; } = true;

		public double TintA { get; set; } = 128.0;

		public double TintB { get; set; } = 128.0;
		
		public int TrimThreshold { get; set; } = 0;
	}
}
