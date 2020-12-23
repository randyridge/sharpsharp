using System;
using System.Globalization;
using NetVips;
using RandyRidge.Common;
using SharpSharp.Pipeline;

namespace SharpSharp {
	internal static class ImageExtensions {
		private const int JpegMaximumResolution = ushort.MaxValue;
		private const double MillimetersInInch = 25.4;
		private const int WebpMaximumResolution = short.MaxValue / 2;

		public static Image Bandbool(this Image image, string boolean) {
			Guard.NotNull(image, nameof(image));
			image = image.Bandbool(boolean);
			return image.Copy(interpretation:Enums.Interpretation.Bw);
		}

		public static Image Blur(this Image image, double sigma = -1.0) {
			Guard.NotNull(image, nameof(image));
			if(!sigma.IsAboutEqualTo(-1.0)) {
				// Slower, accurate Gaussian blur
				return image.Gaussblur(sigma);
			}

			// Fast, mild blur - averages neighboring pixels
			var matrix = new double[3, 3];

			for(var x = 0; x < matrix.GetLength(0); x++) {
				for(var y = 0; y < matrix.GetLength(1); y++) {
					matrix[x, y] = 1.0;
				}
			}

			var blur = Image.NewFromArray(matrix);
			blur.Set("scale", 9.0);
			return image.Conv(blur);
		}

		public static Image Boolean(this Image image, Image imageR, string boolean) {
			Guard.NotNull(image, nameof(image));
			Guard.NotNull(imageR, nameof(imageR));
			return image.Boolean(imageR, boolean);
		}

		public static Image Convolve(this Image image, int width, int height, double scale, double offset, double[] kernel) {
			Guard.NotNull(image, nameof(image));
			var k = Image.NewFromMemory(kernel, width, height, 1, Enums.BandFormat.Double);
			k.Set("scale", scale);
			k.Set("offset", offset);
			return image.Conv(k);
		}

		public static Image EnsureAlpha(this Image image) {
			Guard.NotNull(image, nameof(image));
			return image.HasAlpha() ? image : image.BandjoinConst(new[] {image.Interpretation.MaximumImageAlpha()});
		}

		public static int ExifOrientation(this Image image) {
			Guard.NotNull(image, nameof(image));
			return image.GetTypeOf("orientation") == GValue.GIntType ? Convert.ToInt32(image.Get("orientation"), CultureInfo.InvariantCulture) : 0;
		}

		public static Image Gamma(this Image image, double exponent) {
			Guard.NotNull(image, nameof(image));
			if(image.HasAlpha()) {
				var alpha = image[image.Bands - 1];
				return image
					.RemoveAlpha()
					.Gamma(exponent)
					.Bandjoin(alpha);
			}

			return image.Gamma(exponent);
		}

		public static int GetDensity(this Image image) {
			Guard.NotNull(image, nameof(image));
			return (int) Math.Round(image.Xres * MillimetersInInch, MidpointRounding.AwayFromZero);
		}

		public static bool HasAlpha(this Image image) {
			Guard.NotNull(image, nameof(image));
			var bands = image.Bands;
			var interpretation = image.Interpretation;
			return bands == 2 && interpretation == Enums.Interpretation.Bw ||
			       bands == 4 && interpretation != Enums.Interpretation.Cmyk ||
			       bands == 5 && interpretation == Enums.Interpretation.Cmyk;
		}

		public static bool HasDensity(this Image image) {
			Guard.NotNull(image, nameof(image));
			return image.Xres > 1.0;
		}

		public static bool HasProfile(this Image image) {
			Guard.NotNull(image, nameof(image));
			try {
				return image.Get("icc-profile-data").HasValue();
			}
			catch(VipsException) {
				return false;
			}
		}

		public static bool HasValidDimensions(this Image image, ImageType imageType) {
			Guard.NotNull(image, nameof(image));

			if(imageType == ImageType.Jpeg) {
				return image.Width <= JpegMaximumResolution && image.Height <= JpegMaximumResolution;
			}

			if(imageType == ImageType.WebP) {
				return image.Width <= WebpMaximumResolution && image.Height <= WebpMaximumResolution;
			}

			return true;
		}

		public static Image Linear(this Image image, double a, double b) {
			Guard.NotNull(image, nameof(image));
			if(image.HasAlpha()) {
				var alpha = image[image.Bands - 1];
				return image
					.RemoveAlpha()
					.Linear(new[] {a}, new[] {b})
					.Bandjoin(alpha);
			}

			return image.Linear(new[] {a}, new[] {b});
		}

		public static Image Modulate(this Image image, double brightness, double saturation, int hue) {
			Guard.NotNull(image, nameof(image));
			if(image.HasAlpha()) {
				var alpha = image[image.Bands - 1];
				return image
					.RemoveAlpha()
					.Colourspace(Enums.Interpretation.Lch)
					.Linear(new[] {brightness, saturation, 1}, new[] {0, 0, (double) hue})
					.Bandjoin(alpha);
			}

			return image
				.Colourspace(Enums.Interpretation.Lch)
				.Linear(new[] {brightness, saturation, 1}, new[] {0, 0, (double) hue});
		}

		public static Image Normalize(this Image image) {
			var typeBeforeNormalize = image.Interpretation;
			if(typeBeforeNormalize == Enums.Interpretation.Rgb) {
				typeBeforeNormalize = Enums.Interpretation.Srgb;
			}

			// Convert to LAB colourspace
			var lab = image.Colourspace(Enums.Interpretation.Lab);
			// Extract luminance
			var luminance = lab[0];
			// Find luminance range
			var stats = luminance.Stats();
			var min = stats[0, 0][0];
			var max = stats[1, 0][0];

			if(min.IsAboutEqualTo(max)) {
				return image;
			}

			// Extract chroma
			var chroma = lab.ExtractBand(1, 2);
			// Calculate multiplication factor and addition
			var f = 100.0 / (max - min);
			var a = -(min * f);
			// Scale luminance, join to chroma, convert back to original colorspace
			var normalized = luminance
				.Linear(new[] {f}, new[] {a})
				.Bandjoin(chroma)
				.Colourspace(typeBeforeNormalize);

			// Extract original alpha channel and join to normalized image
			return HasAlpha(image) ? normalized.Bandjoin(image[image.Bands - 1]) : normalized;
		}

		public static Image Recomb(this Image image, double[] matrix) {
			Guard.NotNull(image, nameof(image));
			var m = image.Bands == 3 ?
				Image.NewFromArray(matrix) :
				Image.NewFromArray(new[,] {{matrix[0], matrix[1], matrix[2], 0.0}, {matrix[3], matrix[4], matrix[5], 0.0}, {matrix[6], matrix[7], matrix[8], 0.0}, {0.0, 0.0, 0.0, 1.0}});
			return image
				.Colourspace(Enums.Interpretation.Srgb)
				.Recomb(m);
		}

		public static Image RemoveAlpha(this Image image) {
			Guard.NotNull(image, nameof(image));
			return image.HasAlpha() ? image.ExtractBand(0, image.Bands - 1) : image;
		}

		public static Image RemoveExifOrientation(this Image image) {
			image = Guard.NotNull(image, nameof(image));
			var copy = image.Copy();
			copy.Remove("orientation");
			return copy;
		}

		public static void SetDensity(this Image image, double density) {
			Guard.NotNull(image, nameof(image));
			var pixelsPerMillimeter = density / MillimetersInInch;
			image.Set("Xres", pixelsPerMillimeter);
			image.Set("Yres", pixelsPerMillimeter);
			image.Set("resolution-unit", "in");
		}

		public static void SetExifOrientation(this Image image, int orientation) {
			Guard.NotNull(image, nameof(image));
			image.Set("orientation", orientation);
		}

		public static Image Sharpen(this Image image, double sigma = -1.0, double flat = 0, double jagged = 0) {
			Guard.NotNull(image, nameof(image));
			if(sigma.IsAboutEqualTo(-1.0)) {
				// Fast, mild sharpen
				var m = new[,] {{-1.0, -1.0, -1.0}, {-1.0, 32.0, -1.0}, {-1.0, -1.0, -1.0}};
				var sharpen = Image.NewFromArray(m);
				sharpen.Set("scale", 24);
				return image.Conv(sharpen);
			}

			// Slow, accurate sharpen in LAB color space, with control over flat vs jagged areas
			var colorspaceBeforeSharpen = image.Interpretation;
			if(colorspaceBeforeSharpen == Enums.Interpretation.Rgb) {
				colorspaceBeforeSharpen = Enums.Interpretation.Srgb;
			}

			return image
				.Sharpen(sigma, m1:flat, m2:jagged)
				.Colourspace(colorspaceBeforeSharpen);
		}

		public static Image Threshold(this Image image, double threshold, bool thresholdGrayscale) {
			Guard.NotNull(image, nameof(image));
			return thresholdGrayscale ? image.Colourspace(Enums.Interpretation.Bw) >= threshold : image >= threshold;
		}

		public static Image Tint(this Image image, double a, double b) {
			var typeBeforeTint = image.Interpretation;
			if(typeBeforeTint == Enums.Interpretation.Rgb) {
				typeBeforeTint = Enums.Interpretation.Srgb;
			}

			// Extract luminance
			var luminance = image.Colourspace(Enums.Interpretation.Lab)[0];

			// Create the tinted version by combining the L from the original and the chroma from the tint
			var chroma = new[] {a, b};
			var tinted = luminance
				.Bandjoin(chroma)
				.Copy(interpretation:Enums.Interpretation.Lab)
				.Colourspace(typeBeforeTint);

			// Attach original alpha channel, if any
			return image.HasAlpha() ? tinted.Bandjoin(image[image.Bands - 1]) : tinted;
		}

		public static Image Trim(this Image image, double threshold) {
			Guard.NotNull(image, nameof(image));
			if(image.Width < 3 && image.Height < 3) {
				throw new VipsException("Image to trim must be at least 3*3 pixels.");
			}

			// Top-left pixel provides the background color
			var background = image.ExtractArea(0, 0, 1, 1);
			if(background.HasAlpha()) {
				background = background.Flatten();
			}

			var output = image.FindTrim(threshold, background[0, 0]);
			var left = Convert.ToInt32(output[0], CultureInfo.InvariantCulture);
			var top = Convert.ToInt32(output[1], CultureInfo.InvariantCulture);
			var width = Convert.ToInt32(output[2], CultureInfo.InvariantCulture);
			var height = Convert.ToInt32(output[3], CultureInfo.InvariantCulture);

			if(width == 0 || height == 0) {
				throw new VipsException("Unexpected error while trimming. Try to lower the threshold.");
			}

			return image.ExtractArea(left, top, width, height);
		}
	}
}
