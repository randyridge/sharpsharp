//using System;
//using SharpSharp.Pipeline.Operations;

using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
	public sealed partial class ImagePipeline {
		//        public ImagePipeline Blur(double sigma = -1) {
		//            options.BlurSigma = Guard.RangeInclusive(sigma, 0.3, 1000, nameof(sigma));
		//            return this;
		//        }

		//        public ImagePipeline Boolean(string operand, string operation, object options) => this;

		//        public ImagePipeline Convolve(object kernel) => throw new NotImplementedException();

		public ImagePipeline Flatten() {
			baton.OperationOptions.Flatten = true;
			return this;
		}

		//        public ImagePipeline Flatten(int red, int green, int blue) {
		//            options.Flatten = true;
		//            // TODO: this isn't right
		//            options.FlattenBackground.AddRange(new double[] {red, green, blue});
		//            return this;
		//        }

		//        public ImagePipeline Flip(bool performFlip = true) {
		//            options.Flip = performFlip;
		//            return this;
		//        }

		//        public ImagePipeline Flop(bool performFlop = true) {
		//            options.Flop = performFlop;
		//            return this;
		//        }

		public ImagePipeline Gamma(double gamma = 2.2) {
        baton.OperationOptions.Gamma = Guard.RangeInclusive(gamma, 1, 3, nameof(gamma));
        baton.OperationOptions.GammaOut = gamma;
        return this;
    }

		public ImagePipeline Gamma(double gamma, double gammaOut) {
			baton.OperationOptions.Gamma = Guard.RangeInclusive(gamma, 1, 3, nameof(gamma));
			baton.OperationOptions.GammaOut = Guard.RangeInclusive(gammaOut, 1, 3, nameof(gammaOut));
			return this;
		}

		//        public ImagePipeline Linear(double a = 1.0, double b = 0.0) {
		//            options.LinearA = a;
		//            options.LinearB = b;
		//            return this;
		//        }

		//        public ImagePipeline Median(int size = 3) {
		//            options.MedianSize = Guard.RangeInclusive(size, 1, 1000, nameof(size));
		//            return this;
		//        }

		//        public ImagePipeline Modulate(double brightness, double saturation, int hue) {
		//            options.Brightness = Guard.MinimumExclusive(brightness, 0, nameof(brightness));
		//            options.Saturation = Guard.MinimumExclusive(saturation, 0, nameof(saturation));
		//            options.Hue = hue % 360;
		//            return this;
		//        }

		//        public ImagePipeline Negate(bool performNegation = true) {
		//            options.Negate = performNegation;
		//            return this;
		//        }

		public ImagePipeline Grayscale(bool grayscale = true) {
			baton.OperationOptions.Grayscale = grayscale;
			return this;
		}

		public ImagePipeline Normalize(bool normalize = true) {
			baton.OperationOptions.Normalize = normalize;
			return this;
		}

//        public ImagePipeline Recomb(object matrix) => throw new NotImplementedException();

//        public ImagePipeline Rotate() => AddOperationAndReturn(new ExifOrientationOperation());

		public ImagePipeline Sharpen(double? sigma = null, double? flat = null, double? jagged = null) => Sharpen(new SharpenOptions(sigma, flat, jagged));

		public ImagePipeline Sharpen(SharpenOptions sharpenOptions) {
			Guard.NotNull(sharpenOptions, nameof(sharpenOptions));
			baton.SharpenOptions = sharpenOptions;
			return this;
		}

//        public ImagePipeline Threshold(bool performThreshold) => performThreshold ? Threshold() : Threshold(0);

//        public ImagePipeline Threshold(int threshold = 128, bool thresholdGrayscale = true) {
//            options.Threshold = Guard.RangeInclusive(threshold, 0, 255, nameof(threshold));
//            options.ThresholdGrayscale = thresholdGrayscale;
//            return this;
//        }
	}
}
