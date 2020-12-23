using System;

namespace SharpSharp {
	public sealed record OperationOptions {
		public double[] ConvolveKernel { get; set; } = Array.Empty<double>();
		
		public int ConvolveKernelHeight = 0;

		public double ConvolveKernelOffset = 0;
		
		public double ConvolveKernelScale = 0;
		
		public int ConvolveKernelWidth = 0;

		public double BlurSigma { get; set; } = 0;

		public double Brightness { get; set; } = 1;

		public double[] Composite { get; set; } = Array.Empty<double>();

		public bool Flatten { get; set; } = false;

		public double[] FlattenBackground { get; set; } = {0.0, 0.0, 0.0};

		public double Gamma { get; set; } = 0;

		public double GammaOut { get; set; } = 0;

		public bool Grayscale { get; set; } = false;

		public int Hue { get; set; } = 0;

		public double[] JoinChannelIn { get; set; } = Array.Empty<double>();

		public int MedianSize { get; set; } = 0;

		public bool Negate { get; set; } = false;

		public bool Normalize { get; set; } = false;

		public double[] RecombMatrix { get; set; } = Array.Empty<double>();

		public double Saturation { get; set; } = 1;

		public double SharpenSigma { get; set; } = 0;

		public double SharpenFlat { get; set; } = 1;

		public double SharpenJagged { get; set; } = 2;

		public double Threshold { get; set; } = 0.0;

		public bool ThresholdGrayscale { get; set; } = true;

		public int TrimThreshold { get; set; } = 0;
	}
}
